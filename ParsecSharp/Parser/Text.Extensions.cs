using System.IO;
using System.Text;

namespace Parsec
{
    public static partial class Text
    {
        public static Result<char, T> Parse<T>(this Parser<char, T> parser, string source)
            => parser.Parse(new StringStream(source));

        public static Result<char, T> Parse<T>(this Parser<char, T> parser, Stream source)
            => parser.Parse(new TextStream(source));

        public static Result<char, T> Parse<T>(this Parser<char, T> parser, Stream source, Encoding encoding)
            => parser.Parse(new TextStream(source, encoding));
    }
}
