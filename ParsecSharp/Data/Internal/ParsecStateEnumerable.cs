using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateEnumerable<TToken, TState> : IEnumerable<TState>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _source;

        public ParsecStateEnumerable(TState source)
        {
            this._source = source;
        }

        public IEnumerator<TState> GetEnumerator()
        => new ParsecStateEnumerator<TToken, TState>(this._source);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
