using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<char, T> Parse<T>(this IParser<char, T> parser, string source)
            => parser.Parse(StringStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<char, T> Parse<T>(this IParser<char, T> parser, Stream source)
            => parser.Parse(TextStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<char, T> Parse<T>(this IParser<char, T> parser, Stream source, Encoding encoding)
            => parser.Parse(TextStream.Create(source, encoding));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<char, T> Parse<T>(this IParser<char, T> parser, TextReader source)
            => parser.Parse(TextStream.Create(source));
    }
}
