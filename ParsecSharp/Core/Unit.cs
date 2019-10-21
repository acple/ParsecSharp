using System;

namespace ParsecSharp
{
    public readonly struct Unit : IComparable<Unit>, IEquatable<Unit>
    {
        public static Unit Instance => default;

        public int CompareTo(Unit other)
            => 0;

        public bool Equals(Unit other)
            => true;

        public override bool Equals(object? obj)
            => obj is Unit;

        public override int GetHashCode()
            => 0;

        public override string ToString()
            => $"<{nameof(Unit)}>";

        public static bool operator ==(Unit _0, Unit _1)
            => true;

        public static bool operator !=(Unit _0, Unit _1)
            => false;
    }
}
