using System.Collections;
using System.Collections.Generic;

namespace Parsec.Internal
{
    public static class ParsecStateStreamEnumerable
    {
        public static IEnumerable<TToken> AsEnumerable<TToken>(this IParsecStateStream<TToken> stream)
            => new ParsecStateStreamEnumerable<TToken>(stream);
    }

    internal class ParsecStateStreamEnumerable<TToken> : IEnumerable<TToken>
    {
        private readonly IParsecStateStream<TToken> _stream;

        public ParsecStateStreamEnumerable(IParsecStateStream<TToken> stream)
        {
            this._stream = stream;
        }

        public IEnumerator<TToken> GetEnumerator()
            => new ParsecStateStreamEnumerator<TToken>(this._stream);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
