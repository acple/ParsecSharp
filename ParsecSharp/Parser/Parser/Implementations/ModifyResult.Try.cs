using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Try<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, T> resume) : ModifyResult<TToken, T, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
        => Result.Success<TToken, TState, T>(resume(failure), state);

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
        => success;
}
