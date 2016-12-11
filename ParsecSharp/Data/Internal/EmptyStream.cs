using System.Collections.Generic;
using System.Linq;

namespace Parsec.Internal
{
    public sealed class EmptyStream<TToken> : IParsecStateStream<TToken>
    {
        public static IParsecStateStream<TToken> Instance { get; } = new EmptyStream<TToken>();

        public TToken Current => default(TToken);

        public bool HasValue => false;

        public IPosition Position => LinearPosition.Initial;

        public IParsecStateStream<TToken> Next => this;

        private EmptyStream()
        { }

        public void Dispose()
        { }

        public override string ToString()
            => string.Empty;

        IEnumerator<TToken> IEnumerable<TToken>.GetEnumerator()
            => Enumerable.Empty<TToken>().GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Enumerable.Empty<TToken>().GetEnumerator();
    }
}
