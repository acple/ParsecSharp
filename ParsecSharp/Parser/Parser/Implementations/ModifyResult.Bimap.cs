using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Bimap<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, Func<IFailure<TToken, TIntermediate>, T> result) : ModifyResult<TToken, TIntermediate, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, TIntermediate> failure)
        => Result.Success<TToken, TState, T>(result(failure), state);

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, TIntermediate> success)
        => success.Map(function);
}

internal sealed class BimapConst<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, T result) : ModifyResult<TToken, TIntermediate, T>(parser)
{
    protected sealed override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, TIntermediate> failure)
        => Result.Success<TToken, TState, T>(result, state);

    protected sealed override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, TIntermediate> success)
        => success.Map(function);
}
