﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpExamples
{
    // JSONパーサ RFC7159にわりと忠実
    public static class JsonParser
    {
        // stringをパースしてdynamicに詰めて返します。
        public static Result<char, dynamic> Parse(string json)
            => Json().Parse(json);

        // Streamをパースしてdynamicに詰めて返します。
        public static Result<char, dynamic> Parse(Stream json)
            => Json().Parse(json);

        // Streamをパースしてdynamicに詰めて返します。
        // Encoding指定可能オーバーロード。
        public static Result<char, dynamic> Parse(Stream json, Encoding encoding)
            => Json().Parse(json, encoding);

        // パーサをWhiteSpaceで挟み込む拡張メソッド。
        // RFCにはWhiteSpace Charの指定もあったけどUnicodeのSpaces判定でサボり。
        private static Parser<char, T> WithSpaces<T>(this Parser<char, T> parser)
            => parser.Between(Spaces(), Spaces());

        // パース結果をdynamicに詰めるための拡張メソッド。
        private static Parser<TToken, dynamic> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.FMap(x => x as dynamic);

        // JSON Object の開始を表す波括弧にマッチします。
        // begin-object = ws %x7B ws ; == '{'
        private static Parser<char, char> LeftBrace()
            => Char('{').WithSpaces();

        // JSON Object の終了を表す閉じ波括弧にマッチします。
        // end-object = ws %x7D ws ; == }
        private static Parser<char, char> RightBrace()
            => Char('}').WithSpaces();

        // JSON Array の開始を表す開き角括弧にマッチします。
        // begin-array = ws %x5B ws ; == '['
        private static Parser<char, char> LeftBracket()
            => Char('[').WithSpaces();

        // JSON Array の終了を表す閉じ角括弧にマッチします。
        // end-array = ws %x5D ws ; == ']'
        private static Parser<char, char> RightBracket()
            => Char(']').WithSpaces();

        // JSON Object / JSON Array の要素区切りを表すコンマにマッチします。
        // value-separator = ws %x2C ws ; == ','
        private static Parser<char, char> Comma()
            => Char(',').WithSpaces();

        // JSON Object の Key : Value の区切りを表すコロンにマッチします。
        // name-separator = ws %x3A ws ; == ':'
        private static Parser<char, char> Colon()
            => Char(':').WithSpaces();

        // JSON の値にマッチします。
        // value = false / true / null / object / array / number / string
        private static Parser<char, dynamic> Json()
            => Choice(
                JsonObject().AsDynamic(),
                JsonArray().AsDynamic(),
                JsonString().AsDynamic(),
                JsonNumber().AsDynamic(),
                JsonBool().AsDynamic(),
                JsonNull().AsDynamic());

        // JSON Object にマッチします。
        // object = begin-object [ member *( value-separator member ) ] end-object
        private static Parser<char, Dictionary<string, dynamic>> JsonObject()
            => KeyValuePair().SepBy(Comma()).Between(LeftBrace(), RightBrace())
                .FMap(x => x.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2));

        // Key : Value ペアにマッチします。
        // member = string name-separator value
        private static Parser<char, Tuple<string, dynamic>> KeyValuePair()
            => from key in JsonString()
               from _ in Colon()
               from value in Json()
               select new Tuple<string, dynamic>(key, value);

        // JSON Array にマッチします。
        // array = begin-array [ value *( value-separator value ) ] end-array
        private static Parser<char, List<dynamic>> JsonArray()
            => Delay(Json).SepBy(Comma()).Between(LeftBracket(), RightBracket())
                .FMap(x => x.ToList()); // ParseJson()をDelay()で包むことでパーサ構築時の無限再帰を回避

        // JSON String にマッチします。
        // string = quotation-mark *char quotation-mark
        private static Parser<char, string> JsonString()
            => Char('\"').Right(ManyTill(ParseJsonChar(), Char('\"'))).ToStr();

        // JSON String のCharにマッチします。
        // エスケープが必要な文字の置換を行います。
        // 詳細はRFCをみてください。
        private static Parser<char, char> ParseJsonChar()
            => Char('\\').Right(Choice(
                Char('\"').FMap(_ => '\"'),
                Char('\\').FMap(_ => '\\'),
                Char('/').FMap(_ => '/'),
                Char('b').FMap(_ => '\b'),
                Char('f').FMap(_ => '\f'),
                Char('n').FMap(_ => '\n'),
                Char('r').FMap(_ => '\r'),
                Char('t').FMap(_ => '\t'),
                Char('u').Right(HexDigit().Repeat(4).ToStr()
                    .FMap(x => (char)int.Parse(x, NumberStyles.HexNumber)))))
                .Or(Any()); // 本当は NonCtrlChar なんだけど、Anyでサボり

        // JSON Number にマッチします。double型を返します。
        // number = [ minus ] int [ frac ] [ exp ]
        private static Parser<char, double> JsonNumber()
            => from sign in Sign()
               from integer in Int()
               from frac in Frac()
               from exp in Exp()
               select sign((integer + frac) * Math.Pow(10, exp));

        // JSON Number の符号にマッチします。double型の符号を反転させるFuncを返します。
        // minus = %x2D ; == '-'
        private static Parser<char, Func<double, double>> Sign()
            => Try(Char('-').FMap(_ => new Func<double, double>(x => -x)),
                () => x => x);

        // JSON Number の整数部にマッチします。
        // int = zero / ( digit1-9 *DIGIT )
        private static Parser<char, int> Int()
            => Char('0').ToStr().Or(OneOf("123456789").Append(Many(Digit())).ToStr())
                .FMap(x => int.Parse(x));

        // JSON Number の小数部にマッチします。
        // frac = decimal-point 1*DIGIT
        private static Parser<char, double> Frac()
            => Try(Char('.').Right(Many1(Digit())).ToStr().FMap(x => double.Parse("0." + x)),
                () => 0.0);

        // JSON Number の指数部にマッチします。
        // exp = e [ minus / plus ] 1*DIGIT
        private static Parser<char, int> Exp()
            => Try(from _ in OneOf("eE")
                   from sign in Char('-').ToStr().Or(Optional(Char('+')).FMap(__ => ""))
                   from num in Many1(Digit()).ToStr()
                   select int.Parse(sign + num),
                () => 0);

        // JSON Boolean にマッチします。
        // true  = %x74.72.75.65
        // false = %x66.61.6c.73.65
        private static Parser<char, bool> JsonBool()
            => Choice(
                String("false").FMap(_ => false),
                String("true").FMap(_ => true));

        // JSON Null にマッチします。
        // null = %x6e.75.6c.6c
        private static Parser<char, object> JsonNull()
            => String("null").FMap(_ => null as object);
    }
}
