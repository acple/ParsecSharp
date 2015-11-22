using System;
using System.Linq;
using Parsec.Internal;
using static Parsec.Parser;

namespace Parsec
{
    public static partial class Text
    {
        public static Parser<char, T> Return<T>(T value)
            => Return<char, T>(value);

        public static Parser<char, char> Any()
            => Any<char>();

        public static Parser<char, Unit> EndOfInput()
            => EndOfInput<char>();

        public static Parser<char, char> Char(char character)
            => Satisfy(x => x == character);

        public static Parser<char, char> ControlChar()
            => Satisfy(x => char.IsControl(x));

        public static Parser<char, char> Letter()
            => Satisfy(x => char.IsLetter(x));

        public static Parser<char, char> LetterOrDigit()
            => Satisfy(x => char.IsLetterOrDigit(x));

        public static Parser<char, char> Lower()
            => Satisfy(x => char.IsLower(x));

        public static Parser<char, char> Digit()
            => Satisfy(x => char.IsDigit(x));

        public static Parser<char, char> Number()
            => Satisfy(x => char.IsNumber(x));

        public static Parser<char, char> Separator()
            => Satisfy(x => char.IsSeparator(x));

        public static Parser<char, char> Surrogate()
            => Satisfy(x => char.IsSurrogate(x));

        public static Parser<char, char> Symbol()
            => Satisfy(x => char.IsSymbol(x));

        public static Parser<char, char> Upper()
            => Satisfy(x => char.IsUpper(x));

        public static Parser<char, char> OctDigit()
            => OneOf("01234567");

        public static Parser<char, char> HexDigit()
            => OneOf("0123456789abcdef");

        public static Parser<char, char> WhiteSpace()
            => Satisfy(x => char.IsWhiteSpace(x));

        public static Parser<char, Unit> Spaces()
            => SkipMany(WhiteSpace());

        public static Parser<char, char> NewLine()
            => Char('\n');

        public static Parser<char, char> CrLf()
            => Char('\r').Right(NewLine());

        public static Parser<char, char> EndOfLine()
            => Choice(NewLine(), CrLf());

        public static Parser<char, char> Tab()
            => Char('\t');

        public static Parser<char, char> OneOf(string source)
            => Satisfy(x => source.IndexOf(x) != -1);

        public static Parser<char, Unit> NoneOf(string source)
            => Not(OneOf(source));

        public static Parser<char, string> String(string source)
            => Builder.Create<char, string>(state => (state.Take(source.Length).SequenceEqual(source))
                ? Result.Success(source, state.Advance(source.Length))
                : Result.Fail<char, string>(nameof(String), state));

        public static Parser<char, char> Satisfy(Func<char, bool> predicate)
            => Satisfy<char>(predicate);

        public static Parser<char, Unit> Error()
            => Error<char>();

        public static Parser<char, Unit> Error(string message)
            => Error<char>(message);
    }
}
