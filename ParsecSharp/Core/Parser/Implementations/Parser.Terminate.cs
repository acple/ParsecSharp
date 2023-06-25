using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Terminate<TToken, T>(Func<IParsecState<TToken>, string> message) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(message(state), state);
    }
}
