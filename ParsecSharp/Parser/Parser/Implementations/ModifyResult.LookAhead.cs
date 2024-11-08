namespace ParsecSharp.Internal.Parsers
{
    internal sealed class LookAhead<TToken, T>(IParser<TToken, T> parser) : ModifyResult<TToken, T, T>(parser)
    {
        protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
            => Result.Failure<TToken, TState, T>($"At {nameof(LookAhead<TToken, T>)} -> {failure.ToString()}", state);

        protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
            => Result.Success<TToken, TState, T>(success.Value, state);
    }
}
