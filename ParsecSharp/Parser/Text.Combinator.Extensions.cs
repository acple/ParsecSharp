using System.Collections.Generic;
using System.Linq;

namespace Parsec
{
    public static partial class Text
    {
        public static Parser<char, string> ToStr(this Parser<char, IEnumerable<char>> parser)
            => parser.Map(x => new string(x.ToArray()));

        public static Parser<char, string> ToStr(this Parser<char, IEnumerable<string>> parser)
            => parser.Map(x => string.Join(string.Empty, x));
    }
}
