using System;
using System.Globalization;
using System.IO;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples;

// JSON parser, compliant with RFC8259
public class JsonParser
{
    public static IParser<char, dynamic?> Parser { get; } = CreateParser();

    private static IParser<char, dynamic?> CreateParser()
    {
        // Matches JSON Whitespace and ignores its value.
        // ws = *( %x20 / %x09 / %x0A / %x0D ) ; Space / Horizontal tab / Line feed or New line / Carriage return
        var whitespace = SkipMany(OneOf(" \t\n\r"));

        // Matches the opening curly brace for the start of a JSON Object.
        // begin-object = ws %x7B ws ; == '{'
        var openBrace = Char('{').Between(whitespace);

        // Matches the closing curly brace for the end of a JSON Object.
        // end-object = ws %x7D ws ; == '}'
        var closeBrace = Char('}').Between(whitespace);

        // Matches the opening square bracket for the start of a JSON Array.
        // begin-array = ws %x5B ws ; == '['
        var openBracket = Char('[').Between(whitespace);

        // Matches the closing square bracket for the end of a JSON Array.
        // end-array = ws %x5D ws ; == ']'
        var closeBracket = Char(']').Between(whitespace);

        // Matches the comma that separates elements in a JSON Object or JSON Array.
        // value-separator = ws %x2C ws ; == ','
        var comma = Char(',').Between(whitespace);

        // Matches the colon that separates keys and values in a JSON Object.
        // name-separator = ws %x3A ws ; == ':'
        var colon = Char(':').Between(whitespace);

        // Matches characters that do not need to be escaped.
        // unescaped = %x20-21 / %x23-5B / %x5D-10FFFF ; %x22 == '"', %x5C == '\'
        var jsonUnescapedChar = Any().Except(Char('"'), Char('\\'), Satisfy(x => x <= 0x1F));

        // Matches escaped characters.
        // See RFC8259 for details.
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

        // Matches a single character in a JSON String.
        // char = unescaped / escaped
        var jsonChar = jsonUnescapedChar | jsonEscapedChar;

        // Matches a JSON String.
        // string = quotation-mark *char quotation-mark
        var jsonString = Many(jsonChar).Between(Char('"')).AsString();

        // Matches the sign of a JSON Number.
        // minus = %x2D ; == '-'
        var sign = Optional(Char('-').Map(_ => -1), 1);

        // Matches the integer part of a JSON Number.
        // int = zero / ( digit1-9 *DIGIT )
        var integer = Char('0').Map(_ => 0) | OneOf("123456789").Append(Many(DecDigit())).ToInt();

        // Matches the fractional part of a JSON Number.
        // frac = decimal-point 1*DIGIT
        var frac = Char('.').Append(Many1(DecDigit())).ToDouble();

        // Matches the exponent part of a JSON Number.
        // exp = e [ minus / plus ] 1*DIGIT
        var exp = CharIgnoreCase('e').Right(Optional(OneOf("-+"), '+').Append(Many1(DecDigit())).ToInt());

        // Matches a JSON Number and returns a double.
        // number = [ minus ] int [ frac ] [ exp ]
        var jsonNumber =
            from s in sign
            from i in integer
            from f in Optional(frac, 0.0)
            from e in Optional(exp, 0)
            select s * (i + f) * Math.Pow(10, e);

        // Matches a JSON Boolean.
        // true = %x74.72.75.65
        // false = %x66.61.6c.73.65
        var jsonBool = String("false").Map(_ => false) | String("true").Map(_ => true);

        // Matches JSON Null.
        // null = %x6e.75.6c.6c
        var jsonNull = String("null").Map(_ => null as object);

        // Matches a JSON value.
        var jsonValue = Fix<dynamic?>(jsonValue =>
        {
            // Matches a name : value pair.
            // member = string name-separator value
            var jsonMember =
                from name in jsonString
                from _ in colon
                from value in jsonValue
                select (name, value);

            // Matches a JSON Object.
            // object = begin-object [ member *( value-separator member ) ] end-object
            var jsonObject = jsonMember.SeparatedBy(comma).Between(openBrace, closeBrace)
                .Map(members => members.ToDictionary(x => x.name, x => x.value));

            // Matches a JSON Array.
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

        // Matches JSON Text.
        // JSON-text = ws value ws
        var json = jsonValue.Between(whitespace);

        var parser = json.End();

        return parser;
    }

    // Parses a string and returns it as dynamic.
    public IResult<char, dynamic?> Parse(string json)
        => Parser.Parse(json);

    // Parses a Stream and returns it as dynamic. The text must be encoded in UTF-8.
    public IResult<char, dynamic?> Parse(Stream json)
        => Parser.Parse(json);
}

file static class Extensions
{
    extension<TToken, T>(IParser<TToken, T> parser)
    {
        // Extension method to wrap the parse result as dynamic.
        public IParser<TToken, dynamic?> AsDynamic()
            => parser.Map(x => x as dynamic);
    }
}
