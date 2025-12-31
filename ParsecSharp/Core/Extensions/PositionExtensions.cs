using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static class PositionExtensions
{
    extension<TPosition>(TPosition position)
        where TPosition : IPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(TPosition left, TPosition right)
            => left is null ? right is not null : left.CompareTo(right) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(TPosition left, TPosition right)
            => left is not null && left.CompareTo(right) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(TPosition left, TPosition right)
            => left is null || left.CompareTo(right) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(TPosition left, TPosition right)
            => left is null ? right is null : left.CompareTo(right) >= 0;
    }
}
