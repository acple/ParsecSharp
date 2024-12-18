using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;

namespace ParsecSharp
{
    public static class EnumerableStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, LinearPosition<TToken>> Create<TToken>(IEnumerable<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, LinearPosition<TToken>> Create<TToken>(IEnumerator<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, TPosition> Create<TToken, TPosition>(IEnumerable<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>
            => new(source, position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumerableStream<TToken, TPosition> Create<TToken, TPosition>(IEnumerator<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>
            => new(source, position);
    }
}
