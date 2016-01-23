using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpExamples
{
    // CSVパーサ RFC4180にそこそこ忠実
    public static class CsvParser
    {
        public static Result<char, IEnumerable<string[]>> Parse(string csv)
            => Csv().Parse(csv);

        public static Result<char, IEnumerable<string[]>> Parse(Stream csv)
            => Csv().Parse(csv);

        public static Result<char, IEnumerable<string[]>> Parse(Stream csv, Encoding encoding)
            => Csv().Parse(csv, encoding);

        // COMMA = %x2C ; == ','
        private static Parser<char, char> Comma()
            => Char(',');

        // DQUOTE = %x22 ; == '"'
        private static Parser<char, char> DoubleQuote()
            => Char('\"');

        // TEXTDATA =  %x20-21 / %x23-2B / %x2D-7E ; == CHAR except ( COMMA / DQUOTE / CTL )
        // RFCの定義に従うとASCII文字しか受け付けないので独自拡張
        // ( コンマ / 二重引用符 / 制御文字(改行含む) ) を除く全ての文字に対応
        private static Parser<char, char> TextChar()
            => Any().Except(Comma(), DoubleQuote(), ControlChar());

        // escaped = DQUOTE *( TEXTDATA / COMMA / CR / LF / 2DQUOTE ) DQUOTE
        // 二重引用符で囲まれたフィールド値
        // 改行文字は ( LF / CRLF ) のどちらかのみに対応
        private static Parser<char, string> EscapedField()
            => Many(Choice(TextChar(), EndOfLine(), DoubleQuote().Right(DoubleQuote()))).ToStr()
                .Between(DoubleQuote(), DoubleQuote());

        // non-escaped = *TEXTDATA
        private static Parser<char, string> NonEscapedField()
            => Many(TextChar()).ToStr();

        // field = ( escaped / non-escaped )
        private static Parser<char, string> Field()
            => EscapedField().Or(NonEscapedField());

        // record = field *( COMMA field )
        private static Parser<char, string[]> Record()
            => Field().SepBy1(Comma()).FMap(x => x.ToArray());

        // file = [header CRLF] record *(CRLF record) [CRLF]
        // 定義に従うと、最終行に空の改行が存在する場合に要素0のレコードを読み込んでしまうため、行末の改行文字をRequiredに変更
        // 定義では改行文字は CRLF だけど、 ( LF / CRLF ) に拡張
        private static Parser<char, IEnumerable<string[]>> Csv()
            => Record().EndBy1(EndOfLine());
    }
}
