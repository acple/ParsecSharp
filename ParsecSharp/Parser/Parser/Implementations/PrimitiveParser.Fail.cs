using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Fail<TToken, T> : PrimitiveParser<TToken, T>
    {
        protected sealed override IResult<TToken, T> Run<TState>(TState state)
            => Result.Failure<TToken, TState, T>(state);
    }

    internal sealed class FailWithMessage<TToken, T>(string message) : PrimitiveParser<TToken, T>
    {
        protected sealed override IResult<TToken, T> Run<TState>(TState state)
            => Result.Failure<TToken, TState, T>(message, state);
    }

    internal sealed class FailWithMessageDelayed<TToken, T>(Func<IParsecState<TToken>, string> message) : PrimitiveParser<TToken, T>
    {
        protected sealed override IResult<TToken, T> Run<TState>(TState state)
            => Result.Failure<TToken, TState, T>(message(state), state);
    }
}
