using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    internal static class EnumerableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyCollection<T> Append<T>(this IReadOnlyCollection<T> source, T element)
            => source is CountableEnumerable<T> countable
                ? countable.Append(element)
                : new CountableEnumerable<T>(source.AsEnumerable().Append(element), source.Count + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyCollection<T> Prepend<T>(this IReadOnlyCollection<T> source, T element)
            => source is CountableEnumerable<T> countable
                ? countable.Prepend(element)
                : new CountableEnumerable<T>(source.AsEnumerable().Prepend(element), source.Count + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyCollection<T> Concat<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second)
            => first is CountableEnumerable<T> countable
                ? countable.Concat(second)
                : new CountableEnumerable<T>(first.AsEnumerable().Concat(second), first.Count + second.Count);

        private sealed class CountableEnumerable<T>(IEnumerable<T> items, int count) : IReadOnlyCollection<T>
        {
            public int Count => count;

            public IEnumerator<T> GetEnumerator()
                => items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => this.GetEnumerator();

            public CountableEnumerable<T> Append(T element)
                => new(items.Append(element), count + 1);

            public CountableEnumerable<T> Prepend(T element)
                => new(items.Prepend(element), count + 1);

            public CountableEnumerable<T> Concat(IReadOnlyCollection<T> elements)
                => new(items.Concat(elements), count + elements.Count);
        }
    }
}
