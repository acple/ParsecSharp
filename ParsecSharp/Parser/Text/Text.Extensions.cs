using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParsecSharp;

public static partial class Text
{
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
}
