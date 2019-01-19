using System;

namespace ParsecSharp
{
    public interface IParsecStateStream<out TToken> : IParsecState<TToken>, IDisposable
    {
        IParsecStateStream<TToken> Next { get; }
    }
}
