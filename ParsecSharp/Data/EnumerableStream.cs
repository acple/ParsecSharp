using System;
using System.Collections.Generic;
using Parsec.Internal;

namespace Parsec
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
            }
            catch
            {
                this.Dispose();
            }
            this.Current = (this.HasValue) ? enumerator.Current : default;
            this._next = (this.HasValue)
                ? new Lazy<IParsecStateStream<TToken>>(() => new EnumerableStream<TToken>(enumerator, position.Next()), false)
                : new Lazy<IParsecStateStream<TToken>>(() => EmptyStream<TToken>.Instance, false);
        }

        public void Dispose()
            => this.disposable.Dispose();

        public override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : string.Empty;
    }
}
