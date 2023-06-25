using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Do<TToken, T>(Parser<TToken, T> parser, Action<Failure<TToken, T>> fail, Action<T> succeed) : ModifyResult<TToken, T, T>(parser)
    {
        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
        {
            fail(failure);
            return failure;
        }

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
        {
            succeed(success.Value);
            return success;
        }
    }
}
