using System;
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
        public static Parser<char, dynamic?> Parser { get; } = CreateParser();

        private static Parser<char, dynamic?> CreateParser()
        {
            // JSON Whitespace にマッチし、その値は無視します。
            // ws = *( %x20 / %x09 / %x0A / %x0D ) ; Space / Horizontal tab / Line feed or New line / Carriage return
            var whitespace = SkipMany(OneOf(" \t\n\r"));

            // JSON Object の開始を表す開き波括弧にマッチします。
            // begin-object = ws %x7B ws ; == '{'
            var openBrace = Char('{').Between(whitespace);

            // JSON Object の終了を表す閉じ波括弧にマッチします。
            // end-object = ws %x7D ws ; == '}'
            var closeBrace = Char('}').Between(whitespace);

            // JSON Array の開始を表す開き角括弧にマッチします。
            // begin-array = ws %x5B ws ; == '['
            var openBracket = Char('[').Between(whitespace);

            // JSON Array の終了を表す閉じ角括弧にマッチします。
            // end-array = ws %x5D ws ; == ']'
            var closeBracket = Char(']').Between(whitespace);

            // JSON Object / JSON Array の要素区切りを表すコンマにマッチします。
            // value-separator = ws %x2C ws ; == ','
            var comma = Char(',').Between(whitespace);

            // JSON Object の Key : Value の区切りを表すコロンにマッチします。
            // name-separator = ws %x3A ws ; == ':'
            var colon = Char(':').Between(whitespace);

            // エスケープ不要な文字にマッチします。
            // unescaped = %x20-21 / %x23-5B / %x5D-10FFFF ; %x22 == '"', %x5C == '\'
            var jsonUnescapedChar = Any().Except(Char('"'), Char('\\'), Satisfy(x => x <= 0x1F));

            // エスケープされた文字にマッチします。
            // 詳細は RFC8259 をみてください。
            var jsonEscapedChar =
                Char('\\').Right(
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
                            .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo))));

            // JSON String の一文字にマッチします。
            // char = unescaped / escaped
            var jsonChar = jsonUnescapedChar | jsonEscapedChar;

            // JSON String にマッチします。
            // string = quotation-mark *char quotation-mark
            var jsonString = Many(jsonChar).Between(Char('"')).AsString();

            // JSON Number の符号にマッチします。
            // minus = %x2D ; == '-'
            var sign = Optional(Char('-').Map(_ => -1), 1);

            // JSON Number の整数部にマッチします。
            // int = zero / ( digit1-9 *DIGIT )
            var integer = Char('0').Map(_ => 0) | OneOf("123456789").Append(Many(DecDigit())).ToInt();

            // JSON Number の小数部にマッチします。
            // frac = decimal-point 1*DIGIT
            var frac = Char('.').Append(Many1(DecDigit())).ToDouble();

            // JSON Number の指数部にマッチします。
            // exp = e [ minus / plus ] 1*DIGIT
            var exp = CharIgnoreCase('e').Right(Optional(OneOf("-+"), '+').Append(Many1(DecDigit()))).ToInt();

            // JSON Number にマッチします。double を返します。
            // number = [ minus ] int [ frac ] [ exp ]
            var jsonNumber =
                from s in sign
                from i in integer
                from f in Optional(frac, 0.0)
                from e in Optional(exp, 0)
                select s * (i + f) * Math.Pow(10, e);

            // JSON Boolean にマッチします。
            // true = %x74.72.75.65
            // false = %x66.61.6c.73.65
            var jsonBool = String("false").Map(_ => false) | String("true").Map(_ => true);

            // JSON Null にマッチします。
            // null = %x6e.75.6c.6c
            var jsonNull = String("null").Map(_ => null as object);

            // JSON の値にマッチします。
            var jsonValue = Fix<char, dynamic?>(jsonValue =>
            {
                // name : value ペアにマッチします。
                // member = string name-separator value
                var jsonMember =
                    from name in jsonString
                    from _ in colon
                    from value in jsonValue
                    select (name, value);

                // JSON Object にマッチします。
                // object = begin-object [ member *( value-separator member ) ] end-object
                var jsonObject = jsonMember.SeparatedBy(comma).Between(openBrace, closeBrace)
                    .Map(members => members.ToDictionary(x => x.name, x => x.value));

                // JSON Array にマッチします。
                // array = begin-array [ value *( value-separator value ) ] end-array
                var jsonArray = jsonValue.SeparatedBy(comma).Between(openBracket, closeBracket)
                    .ToArray();

                // value = false / true / null / object / array / number / string
                return Choice(
                    jsonString.AsDynamic().AbortIfEntered(),
                    jsonObject.AsDynamic().AbortIfEntered(),
                    jsonArray.AsDynamic().AbortIfEntered(),
                    jsonNumber.AsDynamic().AbortIfEntered(),
                    jsonBool.AsDynamic().AbortIfEntered(),
                    jsonNull.AsDynamic().AbortIfEntered());
            });

            // JSON Text にマッチします。
            // JSON-text = ws value ws
            var json = jsonValue.Between(whitespace);

            var parser = json.End();

            return parser;
        }

        // パース結果を dynamic に詰める拡張メソッド。
        private static Parser<TToken, dynamic?> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x as dynamic);

        // string をパースして dynamic に詰めて返します。
        public static Result<char, dynamic?> Parse(string json)
            => Parser.Parse(json);

        // Stream をパースして dynamic に詰めて返します。テキストは UTF-8 でエンコードされている必要があります。
        public static Result<char, dynamic?> Parse(Stream json)
            => Parser.Parse(json);
    }
}
