#if NETSTANDARD1_0
using System.Collections.Generic;
using System.Linq;

namespace Parsec.Internal
{
    internal static partial class Utility
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
            => source.Concat(new[] { element });

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
            => new[] { element }.Concat(source);
    }
}
#endif
