namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Not<TToken, TIgnore> : ModifyResult<TToken, TIgnore, Unit>
    {
        public Not(Parser<TToken, TIgnore> parser) : base(parser)
        { }

        protected sealed override Result<TToken, Unit> Fail<TState>(TState state, Failure<TToken, TIgnore> failure)
            => Result.Success<TToken, TState, Unit>(Unit.Instance, state);

        protected sealed override Result<TToken, Unit> Succeed<TState>(TState state, Success<TToken, TIgnore> success)
            => Result.Failure<TToken, TState, Unit>($"At {nameof(Not<TToken, TIgnore>)} -> Unexpected succeed '{success.ToString()}'", state);
    }
}
