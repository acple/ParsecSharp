using System;

namespace ParsecSharp;

public partial interface ISuspendedResult<TToken, out T> : IDisposable
{
    public IResult<TToken, T> Result { get; }

    public ISuspendedState<TToken> Rest { get; }
}

public partial interface ISuspendedState<TToken> : IDisposable
{
    public IDisposable? InnerResource { get; }

    public ISuspendedResult<TToken, T> Continue<T>(IParser<TToken, T> parser);
}
