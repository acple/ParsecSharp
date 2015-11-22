using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Parsec
{
    public static partial class Text
    {
        public static Result<char, T> Parse<T>(this Parser<char, T> parser, string source)
            => parser.Parse(new StringStream(source));

        public static Result<char, T> Parse<T>(this Parser<char, T> parser, Stream stream)
            => parser.Parse(new TextStream(stream));

        public static Parser<char, string> ToStr(this Parser<char, IEnumerable<char>> parser)
            => parser.FMap(x => new string(x.ToArray()));

        public static Parser<char, string> ToStr(this Parser<char, char> parser)
            => parser.FMap(x => new string(x, 1));
    }
}
