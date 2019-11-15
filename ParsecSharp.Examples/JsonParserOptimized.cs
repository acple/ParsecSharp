using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // 構築済みパーサをフィールド等にキャッシュして再利用することで実行速度を改善できます。
    // 未初期化のパーサを参照してしまう危険があるため構築難易度が上がります。
    // 静的フィールドの初期化順序はクラス内での定義順となるため、
    // 前方参照を行いたい場合は Delay コンビネータで参照を保持するようにしてください。
    // また、パーサが構築時に副作用を持つ場合も意図しない結果となるため回避してください。
    // ライブラリ提供のパーサ/コンビネータは一切の副作用を持ちません。
    public static class JsonParserOptimized
    {
        private static Parser<TToken, dynamic?> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x as dynamic);

        private static readonly Parser<char, Unit> WhiteSpace =
            SkipMany(OneOf(" \t\n\r"));

        private static readonly Parser<char, char> OpenBrace =
            Char('{').Between(WhiteSpace);

        private static readonly Parser<char, char> CloseBrace =
            Char('}').Between(WhiteSpace);

        private static readonly Parser<char, char> OpenBracket =
            Char('[').Between(WhiteSpace);

        private static readonly Parser<char, char> CloseBracket =
            Char(']').Between(WhiteSpace);

        private static readonly Parser<char, char> Comma =
            Char(',').Between(WhiteSpace);

        private static readonly Parser<char, char> Colon =
            Char(':').Between(WhiteSpace);

        private static readonly Parser<char, dynamic?> JsonValue =
            Choice(
                Delay(() => JsonString).AsDynamic(),
                Delay(() => JsonObject).AsDynamic(),
                Delay(() => JsonArray).AsDynamic(),
                Delay(() => JsonNumber).AsDynamic(),
                Delay(() => JsonBool).AsDynamic(),
                Delay(() => JsonNull).AsDynamic());

        private static readonly Parser<char, char> JsonUnescapedChar =
            Any().Except(Char('"'), Char('\\'), Satisfy(x => x <= 0x1F));

        private static readonly Parser<char, char> JsonEscapedChar =
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
                        .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber))));

        private static readonly Parser<char, char> JsonChar =
            JsonUnescapedChar | JsonEscapedChar;

        private static readonly Parser<char, string> JsonString =
            Many(JsonChar).Between(Char('"')).AsString();

        private static readonly Parser<char, int> Sign =
            Optional(Char('-').Map(_ => -1), 1);

        private static readonly Parser<char, int> Int =
            Char('0').Map(_ => 0) | OneOf("123456789").Append(Many(DecDigit())).ToInt();

        private static readonly Parser<char, double> Frac =
            Char('.').Right(Many1(DecDigit())).AsString()
                .Map(x => double.Parse("0." + x));

        private static readonly Parser<char, int> Exp =
            from _ in CharIgnoreCase('e')
            from sign in Char('-').Or(Optional(Char('+'), '+'))
            from num in Many1(DecDigit()).AsString()
            select int.Parse(sign + num);

        private static readonly Parser<char, double> JsonNumber =
            from sign in Sign
            from integer in Int
            from frac in Optional(Frac, 0.0)
            from exp in Optional(Exp, 0)
            select sign * (integer + frac) * Math.Pow(10, exp);

        private static readonly Parser<char, bool> JsonBool =
            String("false").Map(_ => false) | String("true").Map(_ => true);

        private static readonly Parser<char, object?> JsonNull =
            String("null").Map(_ => null as object);

        private static readonly Parser<char, (string Key, dynamic? Value)> KeyValue =
            from key in JsonString
            from _ in Colon
            from value in JsonValue
            select (key, value);

        private static readonly Parser<char, Dictionary<string, dynamic?>> JsonObject =
            KeyValue.SeparatedBy(Comma).Between(OpenBrace, CloseBrace)
                .Map(members => members.ToDictionary(x => x.Key, x => x.Value));

        private static readonly Parser<char, dynamic?[]> JsonArray =
            JsonValue.SeparatedBy(Comma).Between(OpenBracket, CloseBracket).ToArray();

        public static Parser<char, dynamic?> Json { get; } =
            JsonValue.Between(WhiteSpace).End();

        public static Result<char, dynamic?> Parse(string json)
            => Json.Parse(json);

        public static Result<char, dynamic?> Parse(Stream json)
            => Json.Parse(json);
    }
}
