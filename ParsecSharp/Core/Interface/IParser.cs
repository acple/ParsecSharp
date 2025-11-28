using System;

namespace ParsecSharp;

public interface IParser<TToken, out T>
{
    internal IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        where TState : IParsecState<TToken, TState>;
}
