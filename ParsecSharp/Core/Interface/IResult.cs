using System;

namespace ParsecSharp;

public interface IResult<TToken, out T>
{
    public T Value { get; }

    public TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success);

    public IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function);

    public string ToString();

    internal ISuspendedResult<TToken, T> Suspend();
}

public interface IFailure<TToken, out T> : IResult<TToken, T>
{
    public IParsecState<TToken> State { get; }

    public ParsecException Exception { get; }

    public string Message { get; }

    public IFailure<TToken, TResult> Convert<TResult>();
}

public interface ISuccess<TToken, out T> : IResult<TToken, T>
{
    internal IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont);
}
