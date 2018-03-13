using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;
using static Parsec.Parser;

namespace Parsec
{
    public static partial class Text
    {
        public static Parser<char, char> Any()
            => Any<char>();

        public static Parser<char, Unit> EndOfInput()
            => EndOfInput<char>();

        public static Parser<char, char> Satisfy(Func<char, bool> predicate)
            => Satisfy<char>(predicate);

        public static Parser<char, char> Char(char token)
            => Satisfy(x => x == token);

        public static Parser<char, char> CharIgnoreCase(char token)
            => Satisfy(x => char.ToUpperInvariant(x) == char.ToUpperInvariant(token));

        public static Parser<char, char> Letter()
            => Satisfy(x => char.IsLetter(x));

        public static Parser<char, char> LetterOrDigit()
            => Satisfy(x => char.IsLetterOrDigit(x));

        public static Parser<char, char> Upper()
            => Satisfy(x => char.IsUpper(x));

        public static Parser<char, char> Lower()
            => Satisfy(x => char.IsLower(x));

        public static Parser<char, char> Digit()
            => Satisfy(x => char.IsDigit(x));

        public static Parser<char, char> OctDigit()
            => OneOf("01234567");

        public static Parser<char, char> HexDigit()
            => OneOf("0123456789ABCDEFabcdef");

        public static Parser<char, char> Symbol()
            => Satisfy(x => char.IsSymbol(x));

        public static Parser<char, char> Separator()
            => Satisfy(x => char.IsSeparator(x));

        public static Parser<char, char> Punctuation()
            => Satisfy(x => char.IsPunctuation(x));

        public static Parser<char, char> Number()
            => Satisfy(x => char.IsNumber(x));

        public static Parser<char, char> Surrogate()
            => Satisfy(x => char.IsSurrogate(x));

        public static Parser<char, char> HighSurrogate()
            => Satisfy(x => char.IsHighSurrogate(x));

        public static Parser<char, char> LowSurrogate()
            => Satisfy(x => char.IsLowSurrogate(x));

        public static Parser<char, char> ControlChar()
            => Satisfy(x => char.IsControl(x));

        public static Parser<char, char> WhiteSpace()
            => Satisfy(x => char.IsWhiteSpace(x));

        public static Parser<char, Unit> Spaces()
            => SkipMany(WhiteSpace());

        public static Parser<char, char> NewLine()
            => Char('\n');

        public static Parser<char, char> CrLf()
            => Char('\r').Right(NewLine());

        public static Parser<char, char> EndOfLine()
            => NewLine().Alternative(CrLf());

        public static Parser<char, char> Tab()
            => Char('\t');

        public static Parser<char, char> OneOf(string candidates)
            => Satisfy(x => candidates.IndexOf(x) != -1);

        public static Parser<char, char> OneOf(IEnumerable<char> candidates)
            => OneOf<char>(candidates);

        public static Parser<char, char> NoneOf(string candidates)
            => Satisfy(x => candidates.IndexOf(x) == -1);

        public static Parser<char, char> NoneOf(IEnumerable<char> candidates)
            => NoneOf<char>(candidates);

        public static Parser<char, string> String(string text)
            => String(text, StringComparison.Ordinal);

        public static Parser<char, string> StringIgnoreCase(string text)
            => String(text, StringComparison.OrdinalIgnoreCase);

        public static Parser<char, string> String(string text, StringComparison comparison)
            => Builder.Create<char, string>(state =>
                (new string(state.AsEnumerable().Take(text.Length).ToArray()).Equals(text, comparison))
                    ? Result.Success(text, state.Advance(text.Length))
                    : Result.Fail<char, string>(state));
    }
}
