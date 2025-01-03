#if NETSTANDARD2_0
using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

internal static class StringCompatibility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this string source, char value)
        => source.IndexOf(value) != -1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this string source, char value, StringComparison comparisonType)
        => source.IndexOf(value.ToString(), comparisonType) != -1;
}
#endif
