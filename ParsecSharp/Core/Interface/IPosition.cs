using System;

namespace ParsecSharp;

public partial interface IPosition : IComparable<IPosition>, IEquatable<IPosition>
{
    public int Line { get; }

    public int Column { get; }
}

public interface IPosition<in TToken, TPosition> : IPosition
    where TPosition : IPosition<TToken, TPosition>
{
    public TPosition Next(TToken token);
}
