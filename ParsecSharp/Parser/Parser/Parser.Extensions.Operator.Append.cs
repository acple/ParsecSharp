using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Parser
{
    extension<TToken, T>(IParser<TToken, T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyList<T>> operator +(IParser<TToken, T> left, IParser<TToken, T> right)
            => left.Append(right);
    }

    extension<TToken, T>(IParser<TToken, IEnumerable<T>>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, T> left, IParser<TToken, IEnumerable<T>> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, IEnumerable<T>> left, IParser<TToken, T> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, IEnumerable<T>> left, IParser<TToken, IEnumerable<T>> right)
            => left.Append(right);
    }

    extension<TToken, T>(IParser<TToken, IReadOnlyCollection<T>>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, T> left, IParser<TToken, IReadOnlyCollection<T>> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, T> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, IReadOnlyCollection<T>> right)
            => left.Append(right);
    }

    extension<TToken>(IParser<TToken, string>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, string> operator +(IParser<TToken, string> left, IParser<TToken, string> right)
            => left.Append(right);
    }
}
