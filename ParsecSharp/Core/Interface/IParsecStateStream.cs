using System;

namespace ParsecSharp
{
    public interface IParsecStateStream<TToken> : IParsecState<TToken>, IDisposable
    {
        IParsecStateStream<TToken> Next { get; }
    }
}
