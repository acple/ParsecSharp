using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    public static class ParsecStateStreamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParsecStateStream<TToken> Advance<TToken>(this IParsecStateStream<TToken> state, int count)
            => (0 < count && state.HasValue) ? state.Next.Advance(count - 1) : state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TToken> AsEnumerable<TToken>(this IParsecStateStream<TToken> stream)
            => new ParsecStateStreamEnumerable<TToken>(stream);
    }
}
