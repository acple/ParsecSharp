using System.Linq;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Skip<TToken>(int count) : PrimitiveParser<TToken, Unit>
{
    protected sealed override IResult<TToken, Unit> Run<TState>(TState state)
        => ParsecState.AsEnumerable<TToken, TState>(state).Take(count).ToArray() is var result && result.Length == count
            ? Result.Success<TToken, TState, Unit>(Unit.Instance, result.Length == 0 ? state : result.Last().Next)
            : Result.Failure<TToken, TState, Unit>("An input does not have enough length", state);
}
