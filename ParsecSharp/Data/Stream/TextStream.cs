using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ParsecSharp.Data;

namespace ParsecSharp;

public static class TextStream
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TextPosition> Create(Stream source)
        => Create(source, TextPosition.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TextPosition> Create(Stream source, Encoding encoding)
        => Create(source, encoding, TextPosition.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TextPosition> Create(TextReader reader)
        => Create(reader, TextPosition.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TPosition> Create<TPosition>(Stream source, TPosition position)
        where TPosition : IPosition<char, TPosition>, IComparable<TPosition>
        => new(source, position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TPosition> Create<TPosition>(Stream source, Encoding encoding, TPosition position)
        where TPosition : IPosition<char, TPosition>, IComparable<TPosition>
        => new(source, encoding, position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextStream<TPosition> Create<TPosition>(TextReader reader, TPosition position)
        where TPosition : IPosition<char, TPosition>
        => new(reader, position);
}
