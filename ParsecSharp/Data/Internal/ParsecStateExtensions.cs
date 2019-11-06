using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    public static class ParsecStateExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TState Advance<TToken, TState>(this TState state, int count)
            where TState : IParsecState<TToken, TState>
            => (0 < count && state.HasValue) ? state.Next.Advance<TToken, TState>(count - 1) : state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TState> AsEnumerable<TToken, TState>(this TState stream)
            where TState : IParsecState<TToken, TState>
            => new ParsecStateEnumerable<TToken, TState>(stream);
    }
}
