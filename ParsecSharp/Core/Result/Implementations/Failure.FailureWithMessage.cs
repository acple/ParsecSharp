namespace ParsecSharp.Internal.Results
{
    internal sealed class FailureWithMessage<TToken, TState, T> : Failure<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _state;

        public sealed override IParsecState<TToken> State => this._state;

        public sealed override string Message { get; }

        public FailureWithMessage(string message, TState state)
        {
            this._state = state;
            this.Message = message;
        }

        protected internal sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult<TToken, T>.Create(this, this._state);

        public sealed override Failure<TToken, TNext> Convert<TNext>()
            => new FailureWithMessage<TToken, TState, TNext>(this.Message, this._state);
    }
}
