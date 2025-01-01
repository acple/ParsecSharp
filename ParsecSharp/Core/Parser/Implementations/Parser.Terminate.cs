using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Terminate<TToken, T>(Func<IParsecState<TToken>, string> message) : IParser<TToken, T>
{
    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => Result.Failure<TToken, TState, TResult>(message(state), state);
}
