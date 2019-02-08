using System;

namespace ParsecSharp
{
    public interface IPosition : IComparable<IPosition>, IEquatable<IPosition>
    {
        int Line { get; }

        int Column { get; }
    }
}
