using System;

namespace Parsec
{
    public interface IParsecStateStream<out TToken> : IParsecState<TToken>, IDisposable
    {
        IParsecStateStream<TToken> Next { get; }
    }
}
