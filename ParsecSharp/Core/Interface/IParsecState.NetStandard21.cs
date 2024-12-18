#if NET || NETSTANDARD2_1_OR_GREATER
using System;

namespace ParsecSharp;

public partial interface IParsecState<out TToken, TState>
{
    void IDisposable.Dispose()
        => this.InnerResource?.Dispose();
}
#endif
