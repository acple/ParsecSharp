#if NET || NETSTANDARD2_1_OR_GREATER
using System;

namespace ParsecSharp;

public partial interface ISuspendedResult<TToken, out T>
{
    void IDisposable.Dispose()
    {
        this.Rest?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public partial interface ISuspendedState<TToken>
{
    void IDisposable.Dispose()
    {
        this.InnerResource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
#endif
