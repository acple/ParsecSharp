namespace ParsecSharp.Internal.Results
{
    internal sealed class FailureWithMessage<TToken, TState, T>(string message, TState state) : Failure<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        public sealed override IParsecState<TToken> State => state;

        public sealed override string Message => message;

        protected internal sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult.Create(this, state);

        public sealed override Failure<TToken, TNext> Convert<TNext>()
            => new FailureWithMessage<TToken, TState, TNext>(message, state);
    }
}
