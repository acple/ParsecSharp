#if NET || NETSTANDARD2_1_OR_GREATER
using System;

namespace ParsecSharp
{
    public partial interface IPosition
    {
        int IComparable<IPosition>.CompareTo(IPosition? other)
            => Compare(this, other);

        private static int Compare(IPosition left, IPosition? right)
            => right is null
                ? 1 // always greater than null
                : left.Line != right.Line ? left.Line.CompareTo(right.Line) : left.Column.CompareTo(right.Column);

        bool IEquatable<IPosition>.Equals(IPosition? other)
            => other is not null && this.Line == other.Line && this.Column == other.Column && this.GetType() == other.GetType();

        public static bool operator <(IPosition left, IPosition right)
            => left is null ? right is not null : Compare(left, right) < 0;

        public static bool operator >(IPosition left, IPosition right)
            => left is not null && Compare(left, right) > 0;

        public static bool operator <=(IPosition left, IPosition right)
            => left is null || Compare(left, right) <= 0;

        public static bool operator >=(IPosition left, IPosition right)
            => left is null ? right is null : Compare(left, right) >= 0;
    }
}
#endif
