using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp.Data;

public sealed class ByteStream<TPosition> : IParsecState<byte, ByteStream<TPosition>>
    where TPosition : IPosition<byte, TPosition>
{
    private const int MaxBufferSize = 2048;

    private readonly Buffer<byte> _buffer;

    private readonly int _index;

    private readonly TPosition _position;

    public byte Current => this._buffer[this._index];

    public bool HasValue => this._index < this._buffer.Count;

    public IPosition Position => this._position;

    public IDisposable InnerResource { get; }

    public ByteStream<TPosition> Next => this._index < MaxBufferSize - 1
        ? new(this.InnerResource, this._buffer, this._index + 1, this._position.Next(this.Current))
        : new(this.InnerResource, this._buffer.Next, index: 0, this._position.Next(this.Current));

    public ByteStream(Stream source, TPosition position) : this(source, CreateBuffer(source), index: 0, position)
    { }

    private ByteStream(IDisposable source, Buffer<byte> buffer, int index, TPosition position)
    {
        this.InnerResource = source;
        this._buffer = buffer;
        this._index = index;
        this._position = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Buffer<byte> CreateBuffer(Stream stream)
    {
        try
        {
            var buffer = Enumerable.Repeat(stream, MaxBufferSize)
                .Select(stream => stream.ReadByte())
                .TakeWhile(x => x != -1)
                .Select(x => (byte)x)
                .ToArray();
            return new(buffer, buffer.Length == MaxBufferSize ? () => CreateBuffer(stream) : () => Buffer<byte>.Empty);
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    public void Dispose()
        => this.InnerResource.Dispose();

    public bool Equals(ByteStream<TPosition>? other)
        => other is not null && this._buffer == other._buffer && this._index == other._index;

    public sealed override bool Equals(object? obj)
        => obj is ByteStream<TPosition> state && this._buffer == state._buffer && this._index == state._index;

    public sealed override int GetHashCode()
        => this._buffer.GetHashCode() ^ this._index;

    public sealed override string ToString()
        => this.HasValue ? this.Current.ToString() : "<EndOfStream>";
}
