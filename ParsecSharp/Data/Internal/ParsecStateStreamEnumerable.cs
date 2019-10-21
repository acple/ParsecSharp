using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateStreamEnumerable<TToken> : IEnumerable<TToken>
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
