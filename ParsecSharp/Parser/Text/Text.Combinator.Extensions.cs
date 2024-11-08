using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> AsString(this IParser<char, char> parser)
            => parser.Map(x => x.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, string> AsString(this IParser<char, IEnumerable<char>> parser)
            => parser.Map(values => new string(values.ToArray()));

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
    }
}
