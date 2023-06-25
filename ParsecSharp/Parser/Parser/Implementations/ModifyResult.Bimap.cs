using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Bimap<TToken, TIntermediate, T>(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, Func<Failure<TToken, TIntermediate>, T> result) : ModifyResult<TToken, TIntermediate, T>(parser)
    {
        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIntermediate> failure)
            => Result.Success<TToken, TState, T>(result(failure), state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIntermediate> success)
            => success.Map(function);
    }

    internal sealed class BimapConst<TToken, TIntermediate, T>(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, T result) : ModifyResult<TToken, TIntermediate, T>(parser)
    {
        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIntermediate> failure)
            => Result.Success<TToken, TState, T>(result, state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIntermediate> success)
            => success.Map(function);
    }
}
