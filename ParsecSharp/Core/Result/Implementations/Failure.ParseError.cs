namespace ParsecSharp.Internal.Results
{
    internal sealed class ParseError<TToken, TState, T> : Failure<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _state;

        public sealed override IParsecState<TToken> State => this._state.GetState();

        public sealed override string Message => $"Unexpected '{this._state.ToString()}'";

        public ParseError(TState state)
        {
            this._state = state;
        }

        public sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult<TToken, T>.Create(this, this._state);

        public sealed override Failure<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TState, TNext>(this._state);
    }
}
