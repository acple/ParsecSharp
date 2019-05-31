using System;
using System.Collections.Generic;
using System.Linq;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class EnumerableStream<TToken> : IParsecStateStream<TToken>
    {
        private const int MaxBufferSize = 1024;

        private sealed class Buffer : IDisposable
        {
            private readonly IDisposable disposable;

            private readonly Lazy<Buffer> _next;

            public TToken[] Current { get; }

            public Buffer Next => this._next.Value;

            public Buffer(IEnumerator<TToken> enumerator)
            {
                this.disposable = enumerator;
                try
                {
                    this.Current = Enumerable.Repeat(enumerator, MaxBufferSize)
                        .TakeWhile(enumerator => enumerator.MoveNext())
                        .Select(enumerator => enumerator.Current)
                        .ToArray();
                }
                catch
                {
                    this.Dispose();
                    throw;
                }
                finally
                {
                    this._next = new Lazy<Buffer>(() => new Buffer(enumerator));
                }
            }

            public void Dispose()
                => this.disposable.Dispose();
        }

        private readonly Buffer buffer;

        private readonly LinearPosition _position;

        private int Index => this._position.Column % MaxBufferSize;

        public TToken Current => this.buffer.Current[this.Index];

        public bool HasValue => this.Index < this.buffer.Current.Length;

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => new EnumerableStream<TToken>((this.Index == MaxBufferSize - 1) ? this.buffer.Next : this.buffer, this._position.Next());

        public EnumerableStream(IEnumerable<TToken> source) : this(source.GetEnumerator())
        { }

        public EnumerableStream(IEnumerator<TToken> enumerator) : this(new Buffer(enumerator), LinearPosition.Initial)
        { }

        private EnumerableStream(Buffer buffer, LinearPosition position)
        {
            this.buffer = buffer;
            this._position = position;
        }

        public void Dispose()
            => this.buffer.Dispose();

        public bool Equals(IParsecState<TToken> other)
            => other is EnumerableStream<TToken> state && this.buffer == state.buffer && this._position == state._position;

        public sealed override bool Equals(object obj)
            => obj is EnumerableStream<TToken> state && this.buffer == state.buffer && this._position == state._position;

        public sealed override int GetHashCode()
            => this.buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
