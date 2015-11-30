using System.Collections.Generic;

namespace Parsec
{
    internal static class Utils
    {
        internal static IParsecStateStream<T> Advance<T>(this IParsecStateStream<T> state, int count)
            => (1 <= count && state.HasValue)
                ? state.Next.Advance(count - 1)
                : state;

        internal static List<T> Append<T>(this List<T> list, T value)
        {
            list.Add(value);
            return list;
        }
    }
}
