using System;
using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateStreamEnumerator<TToken> : IEnumerator<TToken>
    {
        private readonly IParsecStateStream<TToken> _source;

        private IParsecStateStream<TToken>? current;

        public TToken Current => this.current!.Current;

        object? IEnumerator.Current => this.Current;

        public ParsecStateStreamEnumerator(IParsecStateStream<TToken> stream)
        {
            this._source = stream;
        }

        public bool MoveNext()
            => (this.current = this.current?.Next ?? this._source).HasValue || (this.current = EmptyStream<TToken>.Instance).HasValue;

        void IEnumerator.Reset()
            => throw new NotSupportedException();

        public void Dispose()
            => this.current = EmptyStream<TToken>.Instance;
    }
}
