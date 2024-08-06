using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Try<TToken, T>(Parser<TToken, T> parser, Func<Failure<TToken, T>, T> resume) : ModifyResult<TToken, T, T>(parser)
    {
        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
            => Result.Success<TToken, TState, T>(resume(failure), state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
            => success;
    }
}
