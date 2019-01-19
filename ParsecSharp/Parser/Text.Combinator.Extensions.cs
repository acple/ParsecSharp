using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parsec
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> ToStr(this Parser<char, IEnumerable<char>> parser)
            => parser.Map(x => new string(x.ToArray()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, int> ToInt(this Parser<char, IEnumerable<char>> parser)
            => parser.ToStr().ToInt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, int> ToInt(this Parser<char, string> parser)
            => parser.Bind(digits => (int.TryParse(digits, out var integer)) ? Pure(integer) : Fail<int>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser)
            => parser.Join(string.Empty);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser, string separator)
            => parser.Map(x => string.Join(separator, x));
    }
}
