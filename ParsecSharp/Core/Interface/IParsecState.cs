using System;

namespace ParsecSharp
{
    public interface IParsecState<TToken>
    {
        TToken Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }
    }

    public partial interface IParsecState<TToken, TState> : IParsecState<TToken>, IEquatable<TState>, IDisposable
        where TState : IParsecState<TToken, TState>
    {
        IDisposable? InnerResource { get; }

        TState Next { get; }
    }
}
