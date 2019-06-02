using System.Collections.Generic;
using System.IO;
using System.Text;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // CSV パーサ、RFC4180 にそこそこ忠実
    public static class CsvParser
    {
        // COMMA = %x2C ; == ','
        private static Parser<char, char> Comma()
            => Char(',');

        // DQUOTE = %x22 ; == '"'
        private static Parser<char, char> DoubleQuote()
            => Char('"');

        // TEXTDATA =  %x20-21 / %x23-2B / %x2D-7E ; == CHAR except ( COMMA / DQUOTE / CTL )
        // RFC の定義に従うと ASCII 文字しか受け付けないので独自拡張
        // ( コンマ / 二重引用符 / 制御文字(改行含む) ) を除く全ての文字に対応
        private static Parser<char, char> TextChar()
            => Any().Except(Comma(), DoubleQuote(), ControlChar());

        // escaped = DQUOTE *( TEXTDATA / COMMA / CR / LF / 2DQUOTE ) DQUOTE
        // 二重引用符で囲まれたフィールド値
        // 改行文字は ( LF / CRLF ) のどちらかのみに対応
        private static Parser<char, string> EscapedField()
            => Many(Choice(TextChar(), Comma(), EndOfLine(), DoubleQuote().Right(DoubleQuote())))
                .Between(DoubleQuote())
                .AsString();

        // non-escaped = *TEXTDATA
        private static Parser<char, string> NonEscapedField()
            => Many(TextChar()).AsString();

        // field = ( escaped / non-escaped )
        private static Parser<char, string> Field()
            => EscapedField() | NonEscapedField();

        // record = field *( COMMA field )
        private static Parser<char, string[]> Record()
            => Field().SepBy1(Comma()).ToArray();

        // file = [header CRLF] record *(CRLF record) [CRLF]
        // header は無視して record と同一に扱う
        // 定義に従うと最終行に改行が存在する場合に空文字を唯一の要素としたレコードを読み込んでしまうため、終端の改行を Required に変更
        // 定義では改行文字として CRLF のみを受け付けるものを ( LF / CRLF ) に拡張
        private static Parser<char, IEnumerable<string[]>> Csv()
            => Record().EndBy(EndOfLine()).End();

        // string をパースします。レコードを string[] に詰めて返します。
        public static Result<char, IEnumerable<string[]>> Parse(string csv)
            => Csv().Parse(csv);

        // Stream をパースします。
        public static Result<char, IEnumerable<string[]>> Parse(Stream csv)
            => Csv().Parse(csv);

        // Stream をパースします。ソースの Encoding を指定できます。
        public static Result<char, IEnumerable<string[]>> Parse(Stream csv, Encoding encoding)
            => Csv().Parse(csv, encoding);
    }
}
