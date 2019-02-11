using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // JSONパーサ RFC8259に忠実なつくり
    public static class JsonParser
    {
        // パース結果をdynamicに詰める拡張メソッド。
        private static Parser<TToken, dynamic> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x as dynamic);

        // JSON WhiteSpace にマッチし、その値は無視します。
        // ws = *( %x20 / %x09 / %x0A / %x0D ) ; Space / Horizontal tab / Line feed or New line / Carriage return
        private static Parser<char, Unit> WhiteSpace()
            => SkipMany(Choice(Char(' '), Char('\t'), Char('\n'), Char('\r')));

        // JSON Object の開始を表す開き波括弧にマッチします。
        // begin-object = ws %x7B ws ; == '{'
        private static Parser<char, char> OpenBrace()
            => Char('{').Between(WhiteSpace());

        // JSON Object の終了を表す閉じ波括弧にマッチします。
        // end-object = ws %x7D ws ; == '}'
        private static Parser<char, char> CloseBrace()
            => Char('}').Between(WhiteSpace());

        // JSON Array の開始を表す開き角括弧にマッチします。
        // begin-array = ws %x5B ws ; == '['
        private static Parser<char, char> OpenBracket()
            => Char('[').Between(WhiteSpace());

        // JSON Array の終了を表す閉じ角括弧にマッチします。
        // end-array = ws %x5D ws ; == ']'
        private static Parser<char, char> CloseBracket()
            => Char(']').Between(WhiteSpace());

        // JSON Object / JSON Array の要素区切りを表すコンマにマッチします。
        // value-separator = ws %x2C ws ; == ','
        private static Parser<char, char> Comma()
            => Char(',').Between(WhiteSpace());

        // JSON Object の Key : Value の区切りを表すコロンにマッチします。
        // name-separator = ws %x3A ws ; == ':'
        private static Parser<char, char> Colon()
            => Char(':').Between(WhiteSpace());

        // JSON の値にマッチします。
        // value = false / true / null / object / array / number / string
        private static Parser<char, dynamic> Json()
            => Choice(
                Delay(JsonString).AsDynamic(),
                Delay(JsonObject).AsDynamic(),
                Delay(JsonArray).AsDynamic(),
                Delay(JsonNumber).AsDynamic(),
                Delay(JsonBool).AsDynamic(),
                Delay(JsonNull).AsDynamic());

        // エスケープ不要な文字にマッチします。
        // unescaped = %x20-21 / %x23-5B / %x5D-10FFFF
        private static Parser<char, char> JsonUnescapedChar()
            => Any().Except(Char('"'), Char('\\'), Satisfy(x => x <= 0x1F));

        // エスケープされた文字にマッチします。
        // 詳細はRFCをみてください。
        private static Parser<char, char> JsonEscapedChar()
            => Char('\\').Right(
                Choice(
                    Char('"').Map(_ => '"'),
                    Char('\\').Map(_ => '\\'),
                    Char('/').Map(_ => '/'),
                    Char('b').Map(_ => '\b'),
                    Char('f').Map(_ => '\f'),
                    Char('n').Map(_ => '\n'),
                    Char('r').Map(_ => '\r'),
                    Char('t').Map(_ => '\t'),
                    Char('u').Right(HexDigit().Repeat(4).ToStr())
                        .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber))));

        // JSON String の一文字にマッチします。
        // char = unescaped / escaped
        private static Parser<char, char> JsonChar()
            => JsonUnescapedChar() | JsonEscapedChar();

        // JSON String にマッチします。
        // string = quotation-mark *char quotation-mark
        private static Parser<char, string> JsonString()
            => Many(JsonChar()).Between(Char('"')).ToStr();

        // JSON Number の符号にマッチします。
        // minus = %x2D ; == '-'
        private static Parser<char, int> Sign()
            => Optional(Char('-').Map(_ => -1), 1);

        // JSON Number の整数部にマッチします。
        // int = zero / ( digit1-9 *DIGIT )
        private static Parser<char, int> Int()
            => Char('0').Map(_ => 0) | OneOf("123456789").Append(Many(Digit())).ToInt();

        // JSON Number の小数部にマッチします。
        // frac = decimal-point 1*DIGIT
        private static Parser<char, double> Frac()
            => Char('.').Right(Many1(Digit())).ToStr()
                .Map(x => double.Parse("0." + x));

        // JSON Number の指数部にマッチします。
        // exp = e [ minus / plus ] 1*DIGIT
        private static Parser<char, int> Exp()
            => from _ in OneOf("eE")
               from sign in Char('-').Or(Optional(Char('+'), '+'))
               from num in Many1(Digit()).ToStr()
               select int.Parse(sign + num);

        // JSON Number にマッチします。doubleを返します。
        // number = [ minus ] int [ frac ] [ exp ]
        private static Parser<char, double> JsonNumber()
            => from sign in Sign()
               from integer in Int()
               from frac in Optional(Frac(), 0.0)
               from exp in Optional(Exp(), 0)
               select sign * (integer + frac) * Math.Pow(10, exp);

        // JSON Boolean にマッチします。
        // true  = %x74.72.75.65
        // false = %x66.61.6c.73.65
        private static Parser<char, bool> JsonBool()
            => String("false").Map(_ => false) | String("true").Map(_ => true);

        // JSON Null にマッチします。
        // null = %x6e.75.6c.6c
        private static Parser<char, object> JsonNull()
            => String("null").Map(_ => null as object);

        // Key : Value ペアにマッチします。
        // member = string name-separator value
        private static Parser<char, (string Key, dynamic Value)> KeyValuePair()
            => from key in JsonString()
               from _ in Colon()
               from value in Json()
               select (key, value);

        // JSON Object にマッチします。
        // object = begin-object [ member *( value-separator member ) ] end-object
        private static Parser<char, Dictionary<string, dynamic>> JsonObject()
            => KeyValuePair().SepBy(Comma())
                .Between(OpenBrace(), CloseBrace())
                .Map(list => list.ToDictionary(x => x.Key, x => x.Value));

        // JSON Array にマッチします。
        // array = begin-array [ value *( value-separator value ) ] end-array
        private static Parser<char, dynamic[]> JsonArray()
            => Json().SepBy(Comma()).Between(OpenBracket(), CloseBracket()).ToArray();

        // stringをパースしてdynamicに詰めて返します。
        public static Result<char, dynamic> Parse(string json)
            => Json().Between(WhiteSpace()).Parse(json);

        // Streamをパースしてdynamicに詰めて返します。テキストはUTF-8でエンコードされている必要があります。
        public static Result<char, dynamic> Parse(Stream json)
            => Json().Between(WhiteSpace()).Parse(json);
    }
}
