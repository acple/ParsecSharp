using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

internal static class EnumerableExtensions
{
    extension<T>(IReadOnlyCollection<T> source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<T> Append(T element)
            => source is CountableEnumerable<T> countable
                ? countable.Append(element)
                : new CountableEnumerable<T>(source.AsEnumerable().Append(element), source.Count + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<T> Prepend(T element)
            => source is CountableEnumerable<T> countable
                ? countable.Prepend(element)
                : new CountableEnumerable<T>(source.AsEnumerable().Prepend(element), source.Count + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<T> Concat(IReadOnlyCollection<T> elements)
            => source is CountableEnumerable<T> countable
                ? countable.Concat(elements)
                : new CountableEnumerable<T>(source.AsEnumerable().Concat(elements), source.Count + elements.Count);
    }

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
