using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Do<TToken, T>(IParser<TToken, T> parser, Action<IFailure<TToken, T>> fail, Action<T> succeed) : ModifyResult<TToken, T, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
    {
        fail(failure);
        return failure;
    }

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
    {
        succeed(success.Value);
        return success;
    }
}
