namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Not<TToken, TIgnore, T>(Parser<TToken, TIgnore> parser, T result) : ModifyResult<TToken, TIgnore, T>(parser)
    {
        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIgnore> failure)
            => Result.Success<TToken, TState, T>(result, state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIgnore> success)
            => Result.Failure<TToken, TState, T>($"Unexpected succeed with value '{success.ToString()}'", state);
    }
}
