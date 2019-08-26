using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class EnumerableStream<TToken> : IParsecStateStream<TToken>
    {
        private const int MaxBufferSize = 1024;

        private readonly IDisposable source;

        private readonly Buffer<TToken> _buffer;

        private readonly LinearPosition _position;

        private int Index => this._position.Column % MaxBufferSize;

        public TToken Current => this._buffer[this.Index];

        public bool HasValue => this.Index < this._buffer.Count;

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => new EnumerableStream<TToken>(this.source, (this.Index == MaxBufferSize - 1) ? this._buffer.Next : this._buffer, this._position.Next());

        public EnumerableStream(IEnumerable<TToken> source) : this(source.GetEnumerator())
        { }

        public EnumerableStream(IEnumerator<TToken> enumerator) : this(enumerator, CreateBuffer(enumerator), LinearPosition.Initial)
        { }

        private EnumerableStream(IDisposable source, Buffer<TToken> buffer, LinearPosition position)
        {
            this.source = source;
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

        public void Dispose()
            => this.source.Dispose();

        public bool Equals(IParsecState<TToken> other)
            => other is EnumerableStream<TToken> state && this._buffer == state._buffer && this._position == state._position;

        public sealed override bool Equals(object? obj)
            => obj is EnumerableStream<TToken> state && this._buffer == state._buffer && this._position == state._position;

        public sealed override int GetHashCode()
            => this._buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
