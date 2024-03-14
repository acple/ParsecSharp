using System.Collections.Generic;
using System.IO;
using System.Text;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // CSV パーサ、RFC4180 にそこそこ忠実
    public class CsvParser
    {
        public static Parser<char, IEnumerable<string[]>> Parser { get; } = CreateParser();

        private static Parser<char, IEnumerable<string[]>> CreateParser()
        {
            // COMMA = %x2C ; == ','
            var comma = Char(',');

            // DQUOTE = %x22 ; == '"'
            var dquote = Char('"');

            // TEXTDATA = %x20-21 / %x23-2B / %x2D-7E ; == CHAR except ( COMMA / DQUOTE / CTL )
            // RFC の定義に従うと ASCII 文字しか受け付けないので独自拡張
            // ( コンマ / 二重引用符 / 制御文字(改行含む) ) を除く全ての文字に対応
            var textChar = Any().Except(comma, dquote, ControlChar());

            // escaped = DQUOTE *( TEXTDATA / COMMA / CR / LF / 2DQUOTE ) DQUOTE
            // 二重引用符で囲まれたフィールド値
            // 改行文字は ( LF / CRLF ) のどちらかのみに対応
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

            // file = [header CRLF] record *(CRLF record) [CRLF]
            // header は無視して record と同一に扱う
            // 定義に従うと最終行に改行が存在する場合に空文字を唯一の要素としたレコードを読み込んでしまうため、終端の改行を Required に変更
            // 定義では改行文字として CRLF のみを受け付けるものを ( LF / CRLF ) に拡張
            var csv = record.EndBy(EndOfLine());

            var parser = csv.End();

            return parser;
        }

        public Result<char, IEnumerable<string[]>> Parse(string csv)
            => Parser.Parse(csv);

        public Result<char, IEnumerable<string[]>> Parse(Stream csv)
            => Parser.Parse(csv);

        public Result<char, IEnumerable<string[]>> Parse(Stream csv, Encoding encoding)
            => Parser.Parse(csv, encoding);
    }
}
