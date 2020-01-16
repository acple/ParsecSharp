using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static class ArrayStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayStream<TToken, LinearPosition<TToken>> Create<TToken>(IReadOnlyList<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayStream<TToken, TPosition> Create<TToken, TPosition>(IReadOnlyList<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>, IEquatable<TPosition>
            => new ArrayStream<TToken, TPosition>(source, position);
    }
}

namespace ParsecSharp.Internal
{
    public sealed class ArrayStream<TToken, TPosition> : IParsecState<TToken, ArrayStream<TToken, TPosition>>
        where TPosition : IPosition<TToken, TPosition>, IEquatable<TPosition>
    {
        private readonly IReadOnlyList<TToken> _source;

        private readonly TPosition _position;

        public TToken Current => this._source[this._position.Column];

        public bool HasValue => this._position.Column < this._source.Count;

        public IPosition Position => this._position;

        public IDisposable? InnerResource => default;

        public ArrayStream<TToken, TPosition> Next => new ArrayStream<TToken, TPosition>(this._source, this._position.Next(this.Current));

        public ArrayStream(IReadOnlyList<TToken> source, TPosition position)
        {
            this._source = source;
            this._position = position;
        }

        public IParsecState<TToken> GetState()
            => this;

        public void Dispose()
        { }

        public bool Equals(ArrayStream<TToken, TPosition> other)
            => this._source == other._source && this._position.Equals(other._position);

        public sealed override bool Equals(object? obj)
            => obj is ArrayStream<TToken, TPosition> state && this._source == state._source && this._position.Equals(state._position);

        public sealed override int GetHashCode()
            => this._source.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
