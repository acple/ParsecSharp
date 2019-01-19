using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    internal static partial class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParsecStateStream<TToken> Advance<TToken>(this IParsecStateStream<TToken> state, int count)
            => (0 < count && state.HasValue)
                ? state.Next.Advance(count - 1)
                : state;
    }
}
