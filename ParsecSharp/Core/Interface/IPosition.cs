using System;

namespace ParsecSharp
{
    public partial interface IPosition : IComparable<IPosition>, IEquatable<IPosition>
    {
        int Line { get; }

        int Column { get; }
    }

    public interface IPosition<TToken, TPosition> : IPosition
        where TPosition : IPosition<TToken, TPosition>
    {
        TPosition Next(TToken token);
    }
}
