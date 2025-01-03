using System.IO;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Bytes
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<byte, T> Parse<T>(this IParser<byte, T> parser, Stream source)
        => parser.Parse(ByteStream.Create(source));
}
