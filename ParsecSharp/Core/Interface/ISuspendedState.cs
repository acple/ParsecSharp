using System;

namespace ParsecSharp
{
    public partial interface ISuspendedState<TToken> : IDisposable
    {
        IDisposable? InnerResource { get; }

        SuspendedResult<TToken, T> Continue<T>(Parser<TToken, T> parser);
    }
}
