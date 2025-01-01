using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ParsecSharp.Internal;

namespace ParsecSharp.Data;

public sealed class TextStream<TPosition> : IParsecState<char, TextStream<TPosition>>
    where TPosition : IPosition<char, TPosition>
{
    private const int MaxBufferSize = 2048;

    private readonly Buffer<char> _buffer;

    private readonly int _index;

    private readonly TPosition _position;

    public char Current => this._buffer[this._index];

    public bool HasValue => this._index < this._buffer.Count;

    public IPosition Position => this._position;

    public IDisposable InnerResource { get; }

    public TextStream<TPosition> Next => this._index < MaxBufferSize - 1
        ? new(this.InnerResource, this._buffer, this._index + 1, this._position.Next(this.Current))
        : new(this.InnerResource, this._buffer.Next, index: 0, this._position.Next(this.Current));

    public TextStream(Stream source, TPosition position) : this(source, Encoding.UTF8, position)
    { }

    public TextStream(Stream source, Encoding encoding, TPosition position) : this(new StreamReader(source, encoding), position)
    { }

    public TextStream(TextReader reader, TPosition position) : this(reader, CreateBuffer(reader), index: 0, position)
    { }

    private TextStream(IDisposable source, Buffer<char> buffer, int index, TPosition position)
    {
        this.InnerResource = source;
        this._buffer = buffer;
        this._index = index;
        this._position = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Buffer<char> CreateBuffer(TextReader reader)
    {
        try
        {
            var buffer = Enumerable.Repeat(reader, MaxBufferSize)
                .Select(reader => reader.Read())
                .TakeWhile(x => x != -1)
                .Select(x => (char)x)
                .ToArray();
            return new(buffer, buffer.Length == MaxBufferSize ? () => CreateBuffer(reader) : () => Buffer<char>.Empty);
        }
        catch
        {
            reader.Dispose();
            throw;
        }
    }

    public void Dispose()
        => this.InnerResource.Dispose();

    public bool Equals(TextStream<TPosition>? other)
        => other is not null && this._buffer == other._buffer && this._index == other._index;

    public sealed override bool Equals(object? obj)
        => obj is TextStream<TPosition> state && this._buffer == state._buffer && this._index == state._index;

    public sealed override int GetHashCode()
        => this._buffer.GetHashCode() ^ this._index;

    public sealed override string ToString()
        => this.HasValue ? CharConvert.ToReadableStringWithCharCode(this.Current) : "<EndOfStream>";
}
