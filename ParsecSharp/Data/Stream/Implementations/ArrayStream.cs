using System;
using System.Collections.Generic;

namespace ParsecSharp.Data;

public sealed class ArrayStream<TToken, TPosition> : IParsecState<TToken, ArrayStream<TToken, TPosition>>
    where TPosition : IPosition<TToken, TPosition>
{
    private readonly IReadOnlyList<TToken> _source;

    private readonly int _index;

    private readonly TPosition _position;

    public TToken Current => this._source[this._index];

    public bool HasValue => this._index < this._source.Count;

    public IPosition Position => this._position;

    public IDisposable? InnerResource => default;

    public ArrayStream<TToken, TPosition> Next => new(this._source, this._index + 1, this._position.Next(this.Current));

    public ArrayStream(IReadOnlyList<TToken> source, TPosition position) : this(source, index: 0, position)
    { }

    private ArrayStream(IReadOnlyList<TToken> source, int index, TPosition position)
    {
        this._source = source;
        this._index = index;
        this._position = position;
    }

    public void Dispose()
    { }

    public bool Equals(ArrayStream<TToken, TPosition>? other)
        => other is not null && this._source == other._source && this._index == other._index;

    public sealed override bool Equals(object? obj)
        => obj is ArrayStream<TToken, TPosition> state && this._source == state._source && this._index == state._index;

    public sealed override int GetHashCode()
        => this._source.GetHashCode() ^ this._index;

    public sealed override string ToString()
        => this.HasValue
            ? this.Current?.ToString() ?? string.Empty
            : "<EndOfStream>";
}
