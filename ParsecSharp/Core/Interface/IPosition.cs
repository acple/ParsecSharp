using System;

namespace ParsecSharp
{
    public partial interface IPosition : IComparable<IPosition>, IEquatable<IPosition>
    {
        int Line { get; }

        int Column { get; }
    }
}
