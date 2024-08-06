using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Abort<TToken, T>(Exception exception) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(exception, state);
    }
}
