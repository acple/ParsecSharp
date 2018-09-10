using System;
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
            => parser.Between(Spaces());

        // パース結果をdynamicに詰める拡張メソッド。
        private static Parser<TToken, dynamic> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x as dynamic);

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
            => KeyValuePair().SepBy(Comma())
                .Between(LeftBrace(), RightBrace())
                .Map(list => list.ToDictionary(x => x.Key, x => x.Value));

        // Key : Value ペアにマッチします。
        // member = string name-separator value
        private static Parser<char, (string Key, dynamic Value)> KeyValuePair()
            => from key in JsonString()
               from _ in Colon()
               from value in Json()
               select (key, value);

        // JSON Array にマッチします。
        // array = begin-array [ value *( value-separator value ) ] end-array
        private static Parser<char, dynamic[]> JsonArray()
            => Delay(Json).SepBy(Comma())
                .Between(LeftBracket(), RightBracket())
                .ToArray(); // Json()をDelay()で包むことでパーサ構築時の無限再帰を回避

        // JSON String にマッチします。
        // string = quotation-mark *char quotation-mark
        private static Parser<char, string> JsonString()
            => Char('\"').Right(ManyTill(JsonChar(), Char('\"'))).ToStr();

        // JSON String の一文字にマッチします。
        // エスケープされた文字は置換します。
        // 詳細はRFCをみてください。
        private static Parser<char, char> JsonChar()
            => Char('\\')
                .Right(Choice(
                    Char('\"').Map(_ => '\"'),
                    Char('\\').Map(_ => '\\'),
                    Char('/').Map(_ => '/'),
                    Char('b').Map(_ => '\b'),
                    Char('f').Map(_ => '\f'),
                    Char('n').Map(_ => '\n'),
                    Char('r').Map(_ => '\r'),
                    Char('t').Map(_ => '\t'),
                    Char('u').Right(HexDigit().Repeat(4).ToStr())
                        .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber))))
                .Or(Any().Except(ControlChar()));

        // JSON Number にマッチします。doubleを返します。
        // number = [ minus ] int [ frac ] [ exp ]
        private static Parser<char, double> JsonNumber()
            => from sign in Sign()
               from integer in Int()
               from frac in Optional(Frac(), 0.0)
               from exp in Optional(Exp(), 0)
               select sign((integer + frac) * Math.Pow(10, exp));

        // JSON Number の符号にマッチします。doubleの符号を反転させるFuncを返します。
        // minus = %x2D ; == '-'
        private static Parser<char, Func<double, double>> Sign()
            => Optional(Char('-').Map(_ => (Func<double, double>)(x => -x)), x => x);

        // JSON Number の整数部にマッチします。
        // int = zero / ( digit1-9 *DIGIT )
        private static Parser<char, int> Int()
            => Char('0').ToStr().Or(OneOf("123456789").Append(Many(Digit())).ToStr()).ToInt();

        // JSON Number の小数部にマッチします。
        // frac = decimal-point 1*DIGIT
        private static Parser<char, double> Frac()
            => Char('.').Right(Many1(Digit())).ToStr().Map(x => double.Parse("0." + x));

        // JSON Number の指数部にマッチします。
        // exp = e [ minus / plus ] 1*DIGIT
        private static Parser<char, int> Exp()
            => from _ in OneOf("eE")
               from sign in Char('-').Or(Optional(Char('+'), '+')).ToStr()
               from num in Many1(Digit()).ToStr()
               select int.Parse(sign + num);

        // JSON Boolean にマッチします。
        // true  = %x74.72.75.65
        // false = %x66.61.6c.73.65
        private static Parser<char, bool> JsonBool()
            => String("false").Map(_ => false) | String("true").Map(_ => true);

        // JSON Null にマッチします。
        // null = %x6e.75.6c.6c
        private static Parser<char, object> JsonNull()
            => String("null").Map(_ => null as object);
    }
}
