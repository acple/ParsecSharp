using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // JSON パーサ、RFC8259 に忠実なつくり
    public static class JsonParser
    {
        // パース結果を dynamic に詰める拡張メソッド。
        private static Parser<TToken, dynamic?> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x as dynamic);

        // JSON WhiteSpace にマッチし、その値は無視します。
        // ws = *( %x20 / %x09 / %x0A / %x0D ) ; Space / Horizontal tab / Line feed or New line / Carriage return
        private static Parser<char, Unit> WhiteSpace()
            => SkipMany(OneOf(" \t\n\r"));

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
        private static Parser<char, dynamic?> JsonValue()
            => Choice(
                Delay(JsonString).AsDynamic().AbortIfEntered(),
                Delay(JsonObject).AsDynamic().AbortIfEntered(),
                Delay(JsonArray).AsDynamic().AbortIfEntered(),
                Delay(JsonNumber).AsDynamic().AbortIfEntered(),
                Delay(JsonBool).AsDynamic().AbortIfEntered(),
                Delay(JsonNull).AsDynamic().AbortIfEntered());

        // エスケープ不要な文字にマッチします。
        // unescaped = %x20-21 / %x23-5B / %x5D-10FFFF ; %x22 == '"', %x5C == '\'
        private static Parser<char, char> JsonUnescapedChar()
            => Any().Except(Char('"'), Char('\\'), Satisfy(x => x <= 0x1F));

        // エスケープされた文字にマッチします。
        // 詳細は RFC8259 をみてください。
        private static Parser<char, char> JsonEscapedChar()
            => Char('\\').Right(
                Choice(
                    Char('"'),
                    Char('\\'),
                    Char('/'),
                    Char('b').Map(_ => '\b'),
                    Char('f').Map(_ => '\f'),
                    Char('n').Map(_ => '\n'),
                    Char('r').Map(_ => '\r'),
                    Char('t').Map(_ => '\t'),
                    Char('u').Right(HexDigit().Repeat(4).AsString())
                        .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber))));

        // JSON String の一文字にマッチします。
        // char = unescaped / escaped
        private static Parser<char, char> JsonChar()
            => JsonUnescapedChar() | JsonEscapedChar();

        // JSON String にマッチします。
        // string = quotation-mark *char quotation-mark
        private static Parser<char, string> JsonString()
            => Many(JsonChar()).Between(Char('"')).AsString();

        // JSON Number の符号にマッチします。
        // minus = %x2D ; == '-'
        private static Parser<char, int> Sign()
            => Optional(Char('-').Map(_ => -1), 1);

        // JSON Number の整数部にマッチします。
        // int = zero / ( digit1-9 *DIGIT )
        private static Parser<char, int> Int()
            => Char('0').Map(_ => 0) | OneOf("123456789").Append(Many(DecDigit())).ToInt();

        // JSON Number の小数部にマッチします。
        // frac = decimal-point 1*DIGIT
        private static Parser<char, double> Frac()
            => Char('.').Right(Many1(DecDigit())).AsString()
                .Map(x => double.Parse("0." + x));

        // JSON Number の指数部にマッチします。
        // exp = e [ minus / plus ] 1*DIGIT
        private static Parser<char, int> Exp()
            => from _ in CharIgnoreCase('e')
               from sign in Char('-').Or(Optional(Char('+'), '+'))
               from num in Many1(DecDigit()).AsString()
               select int.Parse(sign + num);

        // JSON Number にマッチします。double を返します。
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
        private static Parser<char, object?> JsonNull()
            => String("null").Map(_ => null as object);

        // Key : Value ペアにマッチします。
        // member = string name-separator value
        private static Parser<char, (string Key, dynamic? Value)> KeyValue()
            => from key in JsonString()
               from _ in Colon()
               from value in JsonValue()
               select (key, value);

        // JSON Object にマッチします。
        // object = begin-object [ member *( value-separator member ) ] end-object
        private static Parser<char, Dictionary<string, dynamic?>> JsonObject()
            => KeyValue().SeparatedBy(Comma()).Between(OpenBrace(), CloseBrace())
                .Map(members => members.ToDictionary(x => x.Key, x => x.Value));

        // JSON Array にマッチします。
        // array = begin-array [ value *( value-separator value ) ] end-array
        private static Parser<char, dynamic?[]> JsonArray()
            => JsonValue().SeparatedBy(Comma()).Between(OpenBracket(), CloseBracket()).ToArray();

        // JSON ドキュメントにマッチします。
        // JSON-text = ws value ws
        private static Parser<char, dynamic?> Json()
            => JsonValue().Between(WhiteSpace()).End();

        // string をパースして dynamic に詰めて返します。
        public static Result<char, dynamic?> Parse(string json)
            => Json().Parse(json);

        // Stream をパースして dynamic に詰めて返します。テキストは UTF-8 でエンコードされている必要があります。
        public static Result<char, dynamic?> Parse(Stream json)
            => Json().Parse(json);
    }
}
