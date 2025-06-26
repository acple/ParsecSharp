using System.IO;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Bytes
{
    extension<T>(IParser<byte, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<byte, T> Parse(Stream source)
            => parser.Parse(ByteStream.Create(source));
    }
}
