using System;
using System.Collections;
using System.Collections.Generic;

namespace Parsec.Internal
{
    public sealed class ParsecStateStreamEnumerator<T> : IEnumerator<T>
    {
        private IParsecStateStream<T> _stream;

        public T Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public ParsecStateStreamEnumerator(IParsecStateStream<T> stream)
        {
            this._stream = stream;
        }

        public bool MoveNext()
        {
            if (!this._stream.HasValue)
                return false;
            this.Current = this._stream.Current;
            this._stream = this._stream.Next;
            return true;
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Current = default(T);
            this._stream = EmptyStream<T>.Instance;
        }
    }
}
