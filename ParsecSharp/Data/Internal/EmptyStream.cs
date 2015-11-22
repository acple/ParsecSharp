using System.Collections.Generic;

namespace Parsec.Internal
{
    public sealed class EmptyStream<T> : IParsecStateStream<T>
    {
        public static IParsecStateStream<T> Instance { get; } = new EmptyStream<T>();

        private EmptyStream()
        { }

        public T Current => default(T);

        public bool HasValue => false;

        public IParsecStateStream<T> Next => this;

        public IPosition Position => LinearPosition.Initial;

        public void Dispose()
        { }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => new ParsecStateStreamEnumerator<T>(this);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => new ParsecStateStreamEnumerator<T>(this);
    }
}
