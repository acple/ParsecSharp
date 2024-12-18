using System;

namespace ParsecSharp.Internal;

public abstract class PrimitiveParser<TToken, T> : IParser<TToken, T>
{
    protected abstract IResult<TToken, T> Run<TState>(TState state)
        where TState : IParsecState<TToken, TState>;

    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => cont(this.Run(state));
}
