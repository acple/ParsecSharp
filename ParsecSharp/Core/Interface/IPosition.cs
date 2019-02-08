using System;

namespace ParsecSharp
{
    public interface IPosition : IEquatable<IPosition>
    {
        int Line { get; }

        int Column { get; }
    }
}
