#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

namespace ParsecSharp.Examples;

internal static class StringCompatibility
{
    extension(string source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(char value)
            => source.Length != 0 && source[0] == value;
    }
}
#endif
