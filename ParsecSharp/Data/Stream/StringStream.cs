using System.Runtime.CompilerServices;
using ParsecSharp.Data;

namespace ParsecSharp;

public static class StringStream
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringStream<TextPosition> Create(string source)
        => Create(source, TextPosition.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringStream<TPosition> Create<TPosition>(string source, TPosition position)
        where TPosition : IPosition<char, TPosition>
        => new(source, position);
}
