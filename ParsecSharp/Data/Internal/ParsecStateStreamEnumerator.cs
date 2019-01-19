using System;
using System.Collections;
using System.Collections.Generic;

namespace Parsec.Internal
{
    public sealed class ParsecStateStreamEnumerator<TToken> : IEnumerator<TToken>
    {
        private IParsecStateStream<TToken> stream;

        public TToken Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public ParsecStateStreamEnumerator(IParsecStateStream<TToken> stream)
        {
            this.stream = stream;
        }

        public bool MoveNext()
        {
            if (!this.stream.HasValue)
                return false;
            this.Current = this.stream.Current;
            this.stream = this.stream.Next;
            return true;
        }

        void IEnumerator.Reset()
            => throw new NotSupportedException();

        public void Dispose()
        {
            this.Current = default;
            this.stream = EmptyStream<TToken>.Instance;
        }
    }
}
