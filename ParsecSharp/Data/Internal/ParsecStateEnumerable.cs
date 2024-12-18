using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal;

public sealed class ParsecStateEnumerable<TToken, TState>(TState source) : IEnumerable<TState>
    where TState : IParsecState<TToken, TState>
{
    public IEnumerator<TState> GetEnumerator()
        => new ParsecStateEnumerator<TToken, TState>(source);

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}
