using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Char(char token)
            => Satisfy(x => x == token);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> CharIgnoreCase(char token)
            => Satisfy(x => char.ToUpperInvariant(x) == char.ToUpperInvariant(token));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> OneOf(string candidates)
            => Satisfy(candidates.Contains);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> OneOfIgnoreCase(string candidates)
            => Satisfy(x => candidates.Contains(x, StringComparison.OrdinalIgnoreCase));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> NoneOf(string candidates)
            => Satisfy(x => !candidates.Contains(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> NoneOfIgnoreCase(string candidates)
            => Satisfy(x => !candidates.Contains(x, StringComparison.OrdinalIgnoreCase));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Letter()
            => Satisfy(char.IsLetter);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> LetterOrDigit()
            => Satisfy(char.IsLetterOrDigit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Upper()
            => Satisfy(char.IsUpper);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Lower()
            => Satisfy(char.IsLower);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Digit()
            => Satisfy(char.IsDigit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> OctDigit()
            => Satisfy(x => (uint)x - '0' <= '7' - '0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> DecDigit()
            => AsciiDigit();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> HexDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)(x | 0x20) - 'a' <= 'f' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> HexUpperDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)x - 'A' <= 'F' - 'A');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> HexLowerDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0' || (uint)x - 'a' <= 'f' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Symbol()
            => Satisfy(char.IsSymbol);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Separator()
            => Satisfy(char.IsSeparator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Punctuation()
            => Satisfy(char.IsPunctuation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Number()
            => Satisfy(char.IsNumber);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Surrogate()
            => Satisfy(char.IsSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> HighSurrogate()
            => Satisfy(char.IsHighSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> LowSurrogate()
            => Satisfy(char.IsLowSurrogate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> SurrogatePair()
            => HighSurrogate().Append(LowSurrogate()).AsString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> ControlChar()
            => Satisfy(char.IsControl);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> WhiteSpace()
            => Satisfy(char.IsWhiteSpace);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Spaces()
            => SkipMany(WhiteSpace());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Spaces1()
            => SkipMany1(WhiteSpace());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> NewLine()
            => Char('\n');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> CrLf()
            => Char('\r').Right(NewLine());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> EndOfLine()
            => NewLine().Alternative(CrLf());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Tab()
            => Char('\t');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Ascii()
            => Satisfy(x => x <= 0x7F);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> AsciiLetter()
            => Satisfy(x => (uint)(x | 0x20) - 'a' <= 'z' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> AsciiUpperLetter()
            => Satisfy(x => (uint)x - 'A' <= 'Z' - 'A');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> AsciiLowerLetter()
            => Satisfy(x => (uint)x - 'a' <= 'z' - 'a');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> AsciiDigit()
            => Satisfy(x => (uint)x - '0' <= '9' - '0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> String(string text)
            => String(text, StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> StringIgnoreCase(string text)
            => String(text, StringComparison.OrdinalIgnoreCase);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> String(string text, StringComparison comparison)
            => new StringParser(text, comparison);
    }
}
