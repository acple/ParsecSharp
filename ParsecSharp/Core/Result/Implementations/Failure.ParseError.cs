namespace ParsecSharp.Internal.Results;

internal sealed class ParseError<TToken, TState, T>(TState state) : Failure<TToken, T>
    where TState : IParsecState<TToken, TState>
{
    public sealed override IParsecState<TToken> State => state;

    public sealed override string Message => $"Unexpected '{state.ToString()}'";

    public sealed override IFailure<TToken, TResult> Convert<TResult>()
        => new ParseError<TToken, TState, TResult>(state);

    public sealed override ISuspendedResult<TToken, T> Suspend()
        => SuspendedResult.Create(this, state);
}
