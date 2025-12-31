namespace ParsecSharp.Internal.Results;

internal sealed class FailureOverrideMessage<TToken, TInternal, T>(IFailure<TToken, TInternal> failure, string message) : Failure<TToken, T>
{
    public sealed override IParsecState<TToken> State => failure.State;

    public sealed override string Message => message;

    protected sealed override IFailure<TToken, TResult> Convert<TResult>()
        => new FailureOverrideMessage<TToken, TInternal, TResult>(failure, message);

    public sealed override ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, failure.Suspend().Rest);
}
