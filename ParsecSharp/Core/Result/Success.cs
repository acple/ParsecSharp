using System;

namespace ParsecSharp.Internal;

public abstract class Success<TToken, T>(T value) : ISuccess<TToken, T>
{
    public T Value => value;

    public TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success)
        => success(this);

    public abstract IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function);

    public abstract IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont);

    public abstract ISuspendedResult<TToken, T> Suspend();

    protected IResult<TToken, TResult> RunNext<TNext, TState, TResult>(Func<T, IParser<TToken, TNext>> next, TState state, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont)
        where TState : IParsecState<TToken, TState>
        => next(value).Run(state, cont);

    public sealed override string ToString()
        => value?.ToString() ?? string.Empty;
}
