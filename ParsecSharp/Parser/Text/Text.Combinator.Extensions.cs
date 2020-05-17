using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> AsString(this Parser<char, char> parser)
            => parser.Map(x => x.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> AsString(this Parser<char, IEnumerable<char>> parser)
            => parser.Map(x => new string(x.ToArray()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, int> ToInt(this Parser<char, IEnumerable<char>> parser)
            => parser.AsString().ToInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, int> ToInt(this Parser<char, string> parser)
            => parser.Bind(value => int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var integer)
                ? Pure(integer)
                : Fail<int>($"Expected digits but was '{value}'"));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, long> ToLong(this Parser<char, IEnumerable<char>> parser)
            => parser.AsString().ToLong();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, long> ToLong(this Parser<char, string> parser)
            => parser.Bind(value => long.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var integer)
                ? Pure(integer)
                : Fail<long>($"Expected digits but was '{value}'"));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, double> ToDouble(this Parser<char, IEnumerable<char>> parser)
            => parser.AsString().ToDouble();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, double> ToDouble(this Parser<char, string> parser)
            => parser.Bind(value => double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var number)
                ? Pure(number)
                : Fail<double>($"Expected number but was '{value}'"));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser)
            => parser.Map(x => string.Concat(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser, string separator)
            => parser.Map(x => string.Join(separator, x));
    }
}
