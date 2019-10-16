#if NETSTANDARD2_1
using System;

namespace ParsecSharp
{
    public partial interface IParsecStateStream<TToken>
    {
        void IDisposable.Dispose()
            => this.InnerResource?.Dispose();
    }
}
#endif
