using System;
using System.Collections;
using System.Collections.Generic;

namespace Parsec.Internal
{
    public sealed class ParsecStateStreamEnumerator<T> : IEnumerator<T>
    {
        private IParsecStateStream<T> stream;

        public T Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public ParsecStateStreamEnumerator(IParsecStateStream<T> stream)
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
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Current = default(T);
            this.stream = EmptyStream<T>.Instance;
        }
    }
}
