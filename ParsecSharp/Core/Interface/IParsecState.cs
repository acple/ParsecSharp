using System;

namespace ParsecSharp;

public interface IParsecState<out TToken>
{
    public TToken Current { get; }

    public bool HasValue { get; }

    public IPosition Position { get; }
}

public partial interface IParsecState<out TToken, TState> : IParsecState<TToken>, IEquatable<TState>, IDisposable
    where TState : IParsecState<TToken, TState>
{
    public IDisposable? InnerResource { get; }

    public TState Next { get; }
}
