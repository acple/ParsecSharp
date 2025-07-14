using System.Linq;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Skip<TToken>(int count) : PrimitiveParser<TToken, Unit>
{
    protected sealed override IResult<TToken, Unit> Run<TState>(TState state)
        => count == 0
            ? Result.Success<TToken, TState, Unit>(Unit.Instance, state)
            : ParsecState.AsEnumerable<TToken, TState>(state).ElementAtOrDefault(count - 1) is { } result
                ? Result.Success<TToken, TState, Unit>(Unit.Instance, result.Next)
                : Result.Failure<TToken, TState, Unit>("An input does not have enough length", state);
}
