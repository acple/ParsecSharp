#if NETSTANDARD2_0
using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

internal static class StringCompatibility
{
    extension(string source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(char value)
            => source.IndexOf(value) != -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(char value, StringComparison comparisonType)
            => source.IndexOf(value.ToString(), comparisonType) != -1;
    }
}
#endif
