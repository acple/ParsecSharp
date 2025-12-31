using System;

namespace ParsecSharp.Internal.Results;

internal sealed class FailureWithException<TToken, TState, T>(Exception exception, TState state) : Failure<TToken, T>
    where TState : IParsecState<TToken, TState>
{
    public sealed override IParsecState<TToken> State => state;

    public sealed override ParsecSharpException Exception => new(this.ToString(), exception);

    public sealed override string Message => $"Exception '{exception.GetType().Name}' occurred: {exception.ToString()}";

    protected sealed override IFailure<TToken, TResult> Convert<TResult>()
        => new FailureWithException<TToken, TState, TResult>(exception, state);

    public sealed override ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, state);
}
