using System;

namespace ParsecSharp
{
    public partial interface IParsecStateStream<TToken> : IParsecState<TToken>, IDisposable
    {
        IDisposable? InnerResource { get; }

        IParsecStateStream<TToken> Next { get; }
    }
}
