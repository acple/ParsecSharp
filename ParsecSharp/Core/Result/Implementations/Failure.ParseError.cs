namespace ParsecSharp.Internal.Results
{
    internal sealed class ParseError<TToken, TState, T>(TState state) : Failure<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        public sealed override IParsecState<TToken> State => state;

        public sealed override string Message => $"Unexpected '{state.ToString()}'";

        protected internal sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult.Create(this, state);

        public sealed override Failure<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TState, TNext>(state);
    }
}
