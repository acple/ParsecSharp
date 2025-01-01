namespace ParsecSharp.Internal.Parsers;

internal sealed class Not<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, T result) : ModifyResult<TToken, TIgnore, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, TIgnore> failure)
        => Result.Success<TToken, TState, T>(result, state);

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, TIgnore> success)
        => Result.Failure<TToken, TState, T>($"Unexpected succeed with value '{success.ToString()}'", state);
}
