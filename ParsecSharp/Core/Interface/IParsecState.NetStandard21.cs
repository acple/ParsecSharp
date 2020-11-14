#if NETSTANDARD2_1
using System;

namespace ParsecSharp
{
    public partial interface IParsecState<out TToken, TState>
    {
        void IDisposable.Dispose()
            => this.InnerResource?.Dispose();
    }
}
#endif
