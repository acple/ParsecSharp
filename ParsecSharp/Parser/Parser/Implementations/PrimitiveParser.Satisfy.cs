using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Satisfy<TToken>(Func<TToken, bool> predicate) : PrimitiveParser<TToken, TToken>
{
    protected sealed override IResult<TToken, TToken> Run<TState>(TState state)
        => state.HasValue && predicate(state.Current)
            ? Result.Success<TToken, TState, TToken>(state.Current, state.Next)
            : Result.Failure<TToken, TState, TToken>(state);
}
