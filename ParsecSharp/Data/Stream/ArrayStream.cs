using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;

namespace ParsecSharp
{
    public static class ArrayStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayStream<TToken, LinearPosition<TToken>> Create<TToken>(IReadOnlyList<TToken> source)
            => Create(source, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayStream<TToken, TPosition> Create<TToken, TPosition>(IReadOnlyList<TToken> source, TPosition position)
            where TPosition : IPosition<TToken, TPosition>
            => new(source, position);
    }
}
