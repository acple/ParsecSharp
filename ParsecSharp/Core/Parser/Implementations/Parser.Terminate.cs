using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Terminate<TToken, T>(Func<IParsecState<TToken>, string> message) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(message(state), state);
    }
}
