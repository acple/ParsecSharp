using System;

namespace ParsecSharp;

public interface IResult<TToken, out T>
{
    T Value { get; }

    TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success);

    IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function);

    string ToString();

    internal ISuspendedResult<TToken, T> Suspend();
}

public interface IFailure<TToken, out T> : IResult<TToken, T>
{
    IParsecState<TToken> State { get; }

    ParsecException Exception { get; }

    string Message { get; }

    IFailure<TToken, TResult> Convert<TResult>();
}

public interface ISuccess<TToken, out T> : IResult<TToken, T>
{
    internal IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont);
}
