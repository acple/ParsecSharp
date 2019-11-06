using System;

namespace ParsecSharp.Internal
{
    public abstract class PrimitiveParser<TToken, T> : Parser<TToken, T>
    {
        protected abstract Result<TToken, T> Run<TState>(TState state)
            where TState : IParsecState<TToken, TState>;

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => cont(this.Run(state));
    }
}
