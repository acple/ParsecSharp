using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Char(char token)
            => Satisfy(x => x == token);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> CharIgnoreCase(char token)
            => Satisfy(x => char.ToUpperInvariant(x) == char.ToUpperInvariant(token));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> OneOf(string candidates)
            => Satisfy(candidates.Contains);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> OneOfIgnoreCase(string candidates)
            => Satisfy(x => candidates.Contains(x, StringComparison.OrdinalIgnoreCase));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> NoneOf(string candidates)
            => Satisfy(x => !candidates.Contains(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> NoneOfIgnoreCase(string candidates)
            => Satisfy(x => !candidates.Contains(x, StringComparison.OrdinalIgnoreCase));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Letter()
            => Satisfy(x => char.IsLetter(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> LetterOrDigit()
            => Satisfy(x => char.IsLetterOrDigit(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Upper()
            => Satisfy(x => char.IsUpper(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Lower()
            => Satisfy(x => char.IsLower(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Digit()
            => Satisfy(x => char.IsDigit(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> OctDigit()
            => Satisfy(x => '0' <= x && x <= '7');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> DecDigit()
            => Satisfy(x => '0' <= x && x <= '9');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HexDigit()
            => Satisfy(x => '0' <= x && x <= '9' || 'A' <= x && x <= 'F' || 'a' <= x && x <= 'f');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Symbol()
            => Satisfy(x => char.IsSymbol(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Separator()
            => Satisfy(x => char.IsSeparator(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Punctuation()
            => Satisfy(x => char.IsPunctuation(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Number()
            => Satisfy(x => char.IsNumber(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Surrogate()
            => Satisfy(x => char.IsSurrogate(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HighSurrogate()
            => Satisfy(x => char.IsHighSurrogate(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> LowSurrogate()
            => Satisfy(x => char.IsLowSurrogate(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> SurrogatePair()
            => HighSurrogate().Append(LowSurrogate()).AsString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> ControlChar()
            => Satisfy(x => char.IsControl(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> WhiteSpace()
            => Satisfy(x => char.IsWhiteSpace(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, Unit> Spaces()
            => SkipMany(WhiteSpace());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, Unit> Spaces1()
            => SkipMany1(WhiteSpace());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> NewLine()
            => Char('\n');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> CrLf()
            => Char('\r').Right(NewLine());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> EndOfLine()
            => NewLine().Alternative(CrLf());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Tab()
            => Char('\t');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> String(string text)
            => String(text, StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> StringIgnoreCase(string text)
            => String(text, StringComparison.OrdinalIgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> String(string text, StringComparison comparison)
            => new StringParser(text, comparison);
    }
}
