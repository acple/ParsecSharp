using System;
using System.Collections.Generic;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class EnumerableStream<TToken> : IParsecStateStream<TToken>
    {
        private readonly IDisposable disposable;

        private readonly LinearPosition _position;

        private readonly Lazy<IParsecStateStream<TToken>> _next;

        public TToken Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => this._next.Value;

        public EnumerableStream(IEnumerable<TToken> source) : this(source.GetEnumerator())
        { }

        public EnumerableStream(IEnumerator<TToken> enumerator) : this(enumerator, LinearPosition.Initial)
        { }

        private EnumerableStream(IEnumerator<TToken> enumerator, LinearPosition position)
        {
            this.disposable = enumerator;
            this._position = position;
            try
            {
                this.HasValue = enumerator.MoveNext();
                this.Current = (this.HasValue) ? enumerator.Current : default;
            }
            catch
            {
                this.HasValue = false;
                this.Dispose();
                throw;
            }
            finally
            {
                this._next = new Lazy<IParsecStateStream<TToken>>(() => new EnumerableStream<TToken>(enumerator, position.Next()), false);
            }
        }

        public void Dispose()
            => this.disposable.Dispose();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : string.Empty;
    }
}
