namespace ParsecSharp.Internal.Results;

internal sealed class FailureWithMessage<TToken, TState, T>(string message, TState state) : Failure<TToken, T>
    where TState : IParsecState<TToken, TState>
{
    public sealed override IParsecState<TToken> State => state;

    public sealed override string Message => message;

    protected sealed override IFailure<TToken, TResult> Convert<TResult>()
        => new FailureWithMessage<TToken, TState, TResult>(message, state);

    public sealed override ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, state);
}
