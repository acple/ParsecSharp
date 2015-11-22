using System;
using System.Collections.Generic;

namespace Parsec
{
    public interface IParsecStateStream<T> : IEnumerable<T>, IDisposable
    {
        T Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }

        IParsecStateStream<T> Next { get; }
    }
}
