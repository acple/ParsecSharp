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
            => Satisfy(char.IsLetter);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> LetterOrDigit()
            => Satisfy(char.IsLetterOrDigit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Upper()
            => Satisfy(char.IsUpper);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Lower()
            => Satisfy(char.IsLower);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Digit()
            => Satisfy(char.IsDigit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> OctDigit()
            => Satisfy(x => (uint)x - '0' <= '7' - '0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> DecDigit()
            => AsciiDigit();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HexDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)(x | 0x20) - 'a' <= 'f' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HexUpperDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)x - 'A' <= 'F' - 'A');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HexLowerDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)x - 'a' <= 'f' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Symbol()
            => Satisfy(char.IsSymbol);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Separator()
            => Satisfy(char.IsSeparator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Punctuation()
            => Satisfy(char.IsPunctuation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Number()
            => Satisfy(char.IsNumber);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> Surrogate()
            => Satisfy(char.IsSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> HighSurrogate()
            => Satisfy(char.IsHighSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> LowSurrogate()
            => Satisfy(char.IsLowSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> SurrogatePair()
            => HighSurrogate().Append(LowSurrogate()).AsString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> ControlChar()
            => Satisfy(char.IsControl);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> WhiteSpace()
            => Satisfy(char.IsWhiteSpace);

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
        public static Parser<char, char> Ascii()
            => Satisfy(x => x <= 0x7F);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> AsciiLetter()
            => Satisfy(x => (uint)(x | 0x20) - 'a' <= 'z' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> AsciiUpperLetter()
            => Satisfy(x => (uint)x - 'A' <= 'Z' - 'A');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> AsciiLowerLetter()
            => Satisfy(x => (uint)x - 'a' <= 'z' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, char> AsciiDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0');

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
