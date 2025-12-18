using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ParsecSharp.Internal.Parsers;
using static ParsecSharp.Parser;

namespace ParsecSharp;

public static class Text
{
    #region Parser Extensions

    extension<T>(IParser<char, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<char, T> Parse(string source)
            => parser.Parse(StringStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<char, T> Parse(Stream source)
            => parser.Parse(TextStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<char, T> Parse(Stream source, Encoding encoding)
            => parser.Parse(TextStream.Create(source, encoding));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<char, T> Parse(TextReader source)
            => parser.Parse(TextStream.Create(source));
    }

    #endregion

    #region Character Matching Primitives

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
    public static IParser<char, char> ControlChar()
        => Satisfy(char.IsControl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> WhiteSpace()
        => Satisfy(char.IsWhiteSpace);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> NewLine()
        => Char('\n');

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

    #endregion

    #region Text Sequence Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> String(string text)
        => String(text, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> StringIgnoreCase(string text)
        => String(text, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> String(string text, StringComparison comparison)
        => new StringParser(text, comparison);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> SurrogatePair()
        => HighSurrogate().Append(LowSurrogate());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> CrLf()
        => Char('\r').Right(NewLine());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> EndOfLine()
        => NewLine().Alternative(CrLf());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Spaces()
        => SkipMany(WhiteSpace());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Spaces1()
        => SkipMany1(WhiteSpace());

    #endregion

    #region Text Transformation Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> AsString(this IParser<char, char> parser)
        => parser.Map(x => x.ToString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> AsString(this IParser<char, IEnumerable<char>> parser)
        => parser.Map(values => new string([.. values]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, int> ToInt(this IParser<char, IEnumerable<char>> parser)
        => parser.AsString().ToInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, int> ToInt(this IParser<char, string> parser)
        => parser.Bind(value => int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure(result)
            : Fail<int>($"Expected digits but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, long> ToLong(this IParser<char, IEnumerable<char>> parser)
        => parser.AsString().ToLong();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, long> ToLong(this IParser<char, string> parser)
        => parser.Bind(value => long.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure(result)
            : Fail<long>($"Expected digits but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, double> ToDouble(this IParser<char, IEnumerable<char>> parser)
        => parser.AsString().ToDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, double> ToDouble(this IParser<char, string> parser)
        => parser.Bind(value => double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure(result)
            : Fail<double>($"Expected number but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> Join(this IParser<char, IEnumerable<string>> parser)
        => parser.Map(string.Concat);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, string> Join(this IParser<char, IEnumerable<string>> parser, string separator)
        => parser.Map(values => string.Join(separator, values));

    #endregion

    #region Type Specialized Primitive Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> Any()
        => Any<char>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, char> Satisfy(Func<char, bool> predicate)
        => Satisfy<char>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, IReadOnlyList<char>> Take(int count)
        => Take<char>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, IReadOnlyCollection<char>> TakeWhile(Func<char, bool> predicate)
        => TakeWhile<char>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, IReadOnlyCollection<char>> TakeWhile1(Func<char, bool> predicate)
        => TakeWhile1<char>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Skip(int count)
        => Skip<char>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> SkipWhile(Func<char, bool> predicate)
        => SkipWhile<char>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> SkipWhile1(Func<char, bool> predicate)
        => SkipWhile1<char>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> EndOfInput()
        => EndOfInput<char>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Null()
        => Null<char>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Condition(bool success)
        => Condition<char>(success);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, Unit> Condition(bool success, string message)
        => Condition<char>(success, message);

    #endregion

    #region Type Specialized Monad Primitive Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Pure<T>(T value)
        => Pure<char, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Pure<T>(Func<IParsecState<char>, T> value)
        => Pure<char, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Fail<T>()
        => Fail<char, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Fail<T>(string message)
        => Fail<char, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Fail<T>(Func<IParsecState<char>, string> message)
        => Fail<char, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Abort<T>(Func<IParsecState<char>, string> message)
        => Abort<char, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Abort<T>(Exception exception)
        => Abort<char, T>(exception);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, IPosition> GetPosition()
        => GetPosition<char>();

    #endregion

    #region Type Specialized Combinator Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<char, T> Fix<T>(Func<IParser<char, T>, IParser<char, T>> function)
        => Parser.Fix(function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<TParameter, IParser<char, T>> Fix<TParameter, T>(Func<Func<TParameter, IParser<char, T>>, TParameter, IParser<char, T>> function)
        => Parser.Fix(function);

    #endregion
}
