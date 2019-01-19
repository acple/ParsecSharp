#if NETSTANDARD1_0
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parsec.Internal
{
    internal static partial class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
            => source.Concat(new[] { element });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
            => new[] { element }.Concat(source);
    }
}
#endif
