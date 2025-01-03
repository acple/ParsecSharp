using System;

namespace ParsecSharp.Data;

public sealed class EmptyPosition<TToken> : IPosition<TToken, EmptyPosition<TToken>>, IComparable<EmptyPosition<TToken>>, IEquatable<EmptyPosition<TToken>>
{
    public static EmptyPosition<TToken> Initial { get; } = new();

    public int Line => 0;

    public int Column => -1;

    private EmptyPosition()
    { }

    public EmptyPosition<TToken> Next(TToken token)
        => this;

    public int CompareTo(IPosition? other)
        => this == other ? 0 : other is null ? 1 : -1; // null < empty < others

    public int CompareTo(EmptyPosition<TToken>? other)
        => other is null ? 1 : 0;

    public bool Equals(IPosition? other)
        => ReferenceEquals(this, other);

    public bool Equals(EmptyPosition<TToken>? other)
        => other is not null;

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj);

    public sealed override int GetHashCode()
        => 0;

    public sealed override string ToString()
        => "Position: none";
}
