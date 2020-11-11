#if NETSTANDARD2_1
using System;

namespace ParsecSharp
{
    public partial interface ISuspendedState<TToken>
    {
        void IDisposable.Dispose()
            => this.InnerResource?.Dispose();
    }
}
#endif
