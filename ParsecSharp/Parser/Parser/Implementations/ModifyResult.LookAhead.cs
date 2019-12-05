namespace ParsecSharp.Internal.Parsers
{
    internal sealed class LookAhead<TToken, T> : ModifyResult<TToken, T, T>
    {
        public LookAhead(Parser<TToken, T> parser) : base(parser)
        { }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
            => Result.Failure<TToken, TState, T>($"At {nameof(LookAhead<TToken, T>)} -> {failure.ToString()}", state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
            => Result.Success<TToken, TState, T>(success.Value, state);
    }
}
