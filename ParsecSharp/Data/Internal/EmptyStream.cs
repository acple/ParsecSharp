using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
            => string.Empty;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => Enumerable.Empty<T>().GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Enumerable.Empty<T>().GetEnumerator();
    }
}
