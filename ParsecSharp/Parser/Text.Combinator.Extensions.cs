using System.Collections.Generic;
using System.Linq;

namespace Parsec
{
    public static partial class Text
    {
        public static Parser<char, string> ToStr(this Parser<char, IEnumerable<char>> parser)
            => parser.Map(x => new string(x.ToArray()));

        public static Parser<char, int> ToInt(this Parser<char, string> parser)
            => parser.Bind(number => (int.TryParse(number, out var integer)) ? Pure(integer) : Fail<int>());

        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser)
            => parser.Join(string.Empty);

        public static Parser<char, string> Join(this Parser<char, IEnumerable<string>> parser, string separator)
            => parser.Map(x => string.Join(separator, x));
    }
}
