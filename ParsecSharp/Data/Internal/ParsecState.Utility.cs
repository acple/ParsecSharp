using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    public static class ParsecState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TState Advance<TToken, TState>(TState state, int count)
            where TState : IParsecState<TToken, TState>
            => (0 < count && state.HasValue) ? Advance<TToken, TState>(state.Next, count - 1) : state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TState> AsEnumerable<TToken, TState>(TState stream)
            where TState : IParsecState<TToken, TState>
            => new ParsecStateEnumerable<TToken, TState>(stream);
    }
}
