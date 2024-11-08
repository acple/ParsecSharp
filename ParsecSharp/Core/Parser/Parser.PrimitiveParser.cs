using System;

namespace ParsecSharp.Internal
{
    public abstract class PrimitiveParser<TToken, T> : Parser<TToken, T>
    {
        protected abstract IResult<TToken, T> Run<TState>(TState state)
            where TState : IParsecState<TToken, TState>;

        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => cont(this.Run(state));
    }
}
