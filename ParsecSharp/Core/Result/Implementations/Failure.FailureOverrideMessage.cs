namespace ParsecSharp.Internal.Results;

internal sealed class FailureOverrideMessage<TToken, T>(IFailure<TToken, T> failure, string message) : Failure<TToken, T>
{
    public sealed override IParsecState<TToken> State => failure.State;

    public sealed override string Message => message;

    public sealed override IFailure<TToken, TResult> Convert<TResult>()
        => new FailureOverrideMessage<TToken, TResult>(failure.Convert<TResult>(), message);

    public sealed override ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, failure.Suspend().Rest);
}
