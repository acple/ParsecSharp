using System;

namespace ParsecSharp;

public partial interface ISuspendedResult<TToken, out T> : IDisposable
{
    IResult<TToken, T> Result { get; }

    ISuspendedState<TToken> Rest { get; }
}

public partial interface ISuspendedState<TToken> : IDisposable
{
    IDisposable? InnerResource { get; }

    ISuspendedResult<TToken, T> Continue<T>(IParser<TToken, T> parser);
}
