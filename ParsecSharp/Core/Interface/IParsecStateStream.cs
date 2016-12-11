using System;
using System.Collections.Generic;

namespace Parsec
{
    public interface IParsecStateStream<out TToken> : IParsecState<TToken>, IEnumerable<TToken>, IDisposable
    {
        IParsecStateStream<TToken> Next { get; }
    }
}
