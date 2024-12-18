using System.IO;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;

namespace ParsecSharp;

public static class ByteStream
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteStream<LinearPosition<byte>> Create(Stream source)
        => Create(source, LinearPosition<byte>.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteStream<TPosition> Create<TPosition>(Stream source, TPosition position)
        where TPosition : IPosition<byte, TPosition>
        => new(source, position);
}
