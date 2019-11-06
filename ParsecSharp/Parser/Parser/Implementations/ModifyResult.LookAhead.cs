namespace ParsecSharp.Internal.Parsers
{
    internal sealed class LookAhead<TToken, T> : ModifyResult<TToken, T, T>
    {
        public LookAhead(Parser<TToken, T> parser) : base(parser)
        { }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Fail<TToken, T> fail)
            => Result.Fail<TToken, TState, T>($"At {nameof(LookAhead<TToken, T>)} -> {fail.ToString()}", state);

        protected sealed override Result<TToken, T> Success<TState>(TState state, Success<TToken, T> success)
            => Result.Success<TToken, TState, T>(success.Value, state);
    }
}
