using System;

namespace ParsecSharp.Internal.Results;

internal sealed class Success<TToken, TState, T>(T value, TState state) : ISuccess<TToken, T>
    where TState : IParsecState<TToken, TState>
{
    public T Value => value;

    public TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success)
        => success(this);

    public IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function)
        => new Success<TToken, TState, TResult>(function(value), state);

    public IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont)
        => next(value).Run(state, cont);

    public ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, state);

    public sealed override string ToString()
        => value?.ToString() ?? string.Empty;
}
