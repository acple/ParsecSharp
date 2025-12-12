using System;
using ParsecSharp.Internal.Results;

namespace ParsecSharp.Internal.Parsers;

internal sealed class OverrideMessage<TToken, T>(IParser<TToken, T> parser, string message) : ModifyResult<TToken, T, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
        => new FailureOverrideMessage<TToken, T>(failure, message);

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
        => success;
}

internal sealed class OverrideMessageDelayed<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message) : ModifyResult<TToken, T, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
        => new FailureOverrideMessage<TToken, T>(failure, message(failure));

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
        => success;
}
