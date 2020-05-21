using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static class EnumerableStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, LinearPosition<TToken>> Create<TToken>(IEnumerable<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, LinearPosition<TToken>> Create<TToken>(IEnumerator<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, TPosition> Create<TToken, TPosition>(IEnumerable<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>
            => new(source, position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, TPosition> Create<TToken, TPosition>(IEnumerator<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>
            => new(source, position);
    }
}

namespace ParsecSharp.Internal
{
    public sealed class EnumerableStream<TToken, TPosition> : IParsecState<TToken, EnumerableStream<TToken, TPosition>>
        where TPosition : IPosition<TToken, TPosition>
    {
        private const int MaxBufferSize = 1024;

        private readonly Buffer<TToken> _buffer;

        private readonly int _index;

        private readonly TPosition _position;

        public TToken Current => this._buffer[this._index];

        public bool HasValue => this._index < this._buffer.Count;

        public IPosition Position => this._position;

        public IDisposable InnerResource { get; }

        public EnumerableStream<TToken, TPosition> Next => this._index == MaxBufferSize - 1
            ? new(this.InnerResource, this._buffer.Next, index: 0, this._position.Next(this.Current))
            : new(this.InnerResource, this._buffer, this._index + 1, this._position.Next(this.Current));

        public EnumerableStream(IEnumerable<TToken> source, TPosition position) : this(source.GetEnumerator(), position)
        { }

        public EnumerableStream(IEnumerator<TToken> enumerator, TPosition position) : this(enumerator, CreateBuffer(enumerator), index: 0, position)
        { }

        private EnumerableStream(IDisposable source, Buffer<TToken> buffer, int index, TPosition position)
        {
            this.InnerResource = source;
            this._buffer = buffer;
            this._index = index;
            this._position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Buffer<TToken> CreateBuffer(IEnumerator<TToken> enumerator)
        {
            try
            {
                var buffer = Enumerable.Repeat(enumerator, MaxBufferSize)
                    .TakeWhile(enumerator => enumerator.MoveNext())
                    .Select(enumerator => enumerator.Current)
                    .ToArray();
                return new(buffer, buffer.Length == MaxBufferSize ? () => CreateBuffer(enumerator) : () => Buffer<TToken>.Empty);
            }
            catch
            {
                enumerator.Dispose();
                throw;
            }
        }

        public IParsecState<TToken> GetState()
            => this;

        public void Dispose()
            => this.InnerResource.Dispose();

        public bool Equals(EnumerableStream<TToken, TPosition>? other)
            => other != null && this._buffer == other._buffer && this._index == other._index;

        public sealed override bool Equals(object? obj)
            => obj is EnumerableStream<TToken, TPosition> state && this._buffer == state._buffer && this._index == state._index;

        public sealed override int GetHashCode()
            => this._buffer.GetHashCode() ^ this._index;

        public sealed override string ToString()
            => this.HasValue
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
