using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Pure<TToken, T>(T value) : PrimitiveParser<TToken, T>
    {
        protected sealed override IResult<TToken, T> Run<TState>(TState state)
            => Result.Success<TToken, TState, T>(value, state);
    }

    internal sealed class PureDelayed<TToken, T>(Func<IParsecState<TToken>, T> value) : PrimitiveParser<TToken, T>
    {
        protected sealed override IResult<TToken, T> Run<TState>(TState state)
            => Result.Success<TToken, TState, T>(value(state), state);
    }
}
