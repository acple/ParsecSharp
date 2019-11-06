using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Pure<TToken, T> : PrimitiveParser<TToken, T>
    {
        private readonly T _value;

        public Pure(T value)
        {
            this._value = value;
        }

        protected sealed override Result<TToken, T> Run<TState>(TState state)
            => Result.Success<TToken, TState, T>(this._value, state);
    }

    internal sealed class PureDelayed<TToken, T> : PrimitiveParser<TToken, T>
    {
        private readonly Func<IParsecState<TToken>, T> _value;

        public PureDelayed(Func<IParsecState<TToken>, T> value)
        {
            this._value = value;
        }

        protected sealed override Result<TToken, T> Run<TState>(TState state)
            => Result.Success<TToken, TState, T>(this._value(state.GetState()), state);
    }
}
