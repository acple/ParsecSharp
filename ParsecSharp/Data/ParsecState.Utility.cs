using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static class ParsecState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TState> AsEnumerable<TToken, TState>(TState stream)
            where TState : IParsecState<TToken, TState>
            => new ParsecStateEnumerable<TToken, TState>(stream);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TokenizedStream<TInput, TState, TToken, LinearPosition<TToken>> Tokenize<TInput, TState, TToken>(TState source, Parser<TInput, TToken> parser)
            where TState : IParsecState<TInput, TState>
            => Tokenize(source, parser, LinearPosition<TToken>.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TokenizedStream<TInput, TState, TToken, TPosition> Tokenize<TInput, TState, TToken, TPosition>(TState source, Parser<TInput, TToken> parser, TPosition position)
            where TState : IParsecState<TInput, TState>
            where TPosition : IPosition<TToken, TPosition>
            => new TokenizedStream<TInput, TState, TToken, TPosition>(source, parser, position);
    }
}
