using System.Collections.Generic;
using System.IO;
using System.Text;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // CSV parser, fairly compliant with RFC4180
    public class CsvParser
    {
        public static IParser<char, IReadOnlyCollection<string[]>> Parser { get; } = CreateParser();

        private static IParser<char, IReadOnlyCollection<string[]>> CreateParser()
        {
            // COMMA = %x2C ; == ','
            var comma = Char(',');

            // DQUOTE = %x22 ; == '"'
            var dquote = Char('"');

            // TEXTDATA = %x20-21 / %x23-2B / %x2D-7E ; == CHAR except ( COMMA / DQUOTE / CTL )
            // According to RFC, only ASCII characters are accepted, but this has been extended to include arbitrary characters.
            // Any characters except (comma / double quote / control characters, including newline)
            var textChar = Any().Except(comma, dquote, ControlChar());

            // escaped = DQUOTE *( TEXTDATA / COMMA / CR / LF / 2DQUOTE ) DQUOTE
            // Field value enclosed in double quotes.
            // Unlike the original definition, supported newline characters are only LF and CRLF.
            var escapedField =
                Many(Choice(textChar, comma, EndOfLine(), dquote.Right(dquote)))
                    .Between(dquote)
                    .AsString();

            // non-escaped = *TEXTDATA
            var nonEscapedField = Many(textChar).AsString();

            // field = ( escaped / non-escaped )
            var field = escapedField | nonEscapedField;

            // record = field *( COMMA field )
            var record = field.SeparatedBy1(comma).ToArray();

            // file = [header CRLF] record *( CRLF record ) [CRLF]
            // In this implementation the header and record are treated as the same.
            // Unlike the RFC definition, this requires trailing newline at the last record of the input.
            // *( record ( LF / CRLF ) )
            var csv = record.EndBy(EndOfLine());

            var parser = csv.End();

            return parser;
        }

        public IResult<char, IReadOnlyCollection<string[]>> Parse(string csv)
            => Parser.Parse(csv);

        public IResult<char, IReadOnlyCollection<string[]>> Parse(Stream csv)
            => Parser.Parse(csv);

        public IResult<char, IReadOnlyCollection<string[]>> Parse(Stream csv, Encoding encoding)
            => Parser.Parse(csv, encoding);
    }
}
