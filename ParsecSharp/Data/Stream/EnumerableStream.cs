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
            where TPosition : IPosition<TToken, TPosition>, IComparable<TPosition>, IEquatable<TPosition>
            => new EnumerableStream<TToken, TPosition>(source, position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, TPosition> Create<TToken, TPosition>(IEnumerator<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>, IComparable<TPosition>, IEquatable<TPosition>
            => new EnumerableStream<TToken, TPosition>(source, position);
    }
}

namespace ParsecSharp.Internal
{
    public sealed class EnumerableStream<TToken, TPosition> : IParsecState<TToken, EnumerableStream<TToken, TPosition>>
        where TPosition : IPosition<TToken, TPosition>, IComparable<TPosition>, IEquatable<TPosition>
    {
        private const int MaxBufferSize = 1024;

        private readonly Buffer<TToken> _buffer;

        private readonly TPosition _position;

        private int Index => this._position.Column % MaxBufferSize;

        public TToken Current => this._buffer[this.Index];

        public bool HasValue => this.Index < this._buffer.Count;

        public IPosition Position => this._position;

        public IDisposable InnerResource { get; }

        public EnumerableStream<TToken, TPosition> Next => new EnumerableStream<TToken, TPosition>(
            this.InnerResource,
            (this.Index == MaxBufferSize - 1) ? this._buffer.Next : this._buffer,
            this._position.Next(this.Current));

        public EnumerableStream(IEnumerable<TToken> source, TPosition position) : this(source.GetEnumerator(), position)
        { }

        public EnumerableStream(IEnumerator<TToken> enumerator, TPosition position) : this(enumerator, CreateBuffer(enumerator), position)
        { }

        private EnumerableStream(IDisposable source, Buffer<TToken> buffer, TPosition position)
        {
            this.InnerResource = source;
            this._buffer = buffer;
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
                return new Buffer<TToken>(buffer, () => CreateBuffer(enumerator));
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

        public bool Equals(EnumerableStream<TToken, TPosition> other)
            => this._buffer == other._buffer && this._position.Equals(other._position);

        public sealed override bool Equals(object? obj)
            => obj is EnumerableStream<TToken, TPosition> state && this._buffer == state._buffer && this._position.Equals(state._position);

        public sealed override int GetHashCode()
            => this._buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
