#if NETSTANDARD2_1
using System;

namespace ParsecSharp
{
    public partial interface IPosition
    {
        int IComparable<IPosition>.CompareTo(IPosition? other)
            => (other == null)
                ? 1 // always greater than null
                : (this.Line != other.Line) ? this.Line.CompareTo(other.Line) : this.Column.CompareTo(other.Column);

        bool IEquatable<IPosition>.Equals(IPosition? other)
            => other != null && this.GetType() == other.GetType() && this.Line == other.Line && this.Column == other.Column;

        public static bool operator <(IPosition left, IPosition right)
            => left.CompareTo(right) < 0;

        public static bool operator >(IPosition left, IPosition right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(IPosition left, IPosition right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(IPosition left, IPosition right)
            => left.CompareTo(right) >= 0;
    }
}
#endif
