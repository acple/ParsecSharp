namespace ParsecSharp.Internal.Results
{
    internal sealed class FailWithMessage<TToken, TState, T> : Fail<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _state;

        public sealed override IParsecState<TToken> State => this._state.GetState();

        public sealed override string Message { get; }

        public FailWithMessage(string message, TState state)
        {
            this._state = state;
            this.Message = message;
        }

        public sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult<TToken, T>.Create(this, this._state);

        public sealed override Fail<TToken, TNext> Convert<TNext>()
            => new FailWithMessage<TToken, TState, TNext>(this.Message, this._state);
    }
}
