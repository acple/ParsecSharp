using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CaseOf<TToken, T>(this Result<TToken, T> result, Action<Failure<TToken, T>> failure, Action<Success<TToken, T>> success)
            => result.CaseOf<object?>(
                x => { failure(x); return default; },
                x => { success(x); return default; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action)
            => parser.Map(x => { action(x); return x; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action, Action<Failure<TToken, T>> failure)
            => new Do<TToken, T>(parser, failure, action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TokenizedStream<TInput, TState, TToken> Tokenize<TInput, TState, TToken>(this TState source, Parser<TInput, TToken> parser)
            where TState : IParsecState<TInput, TState>
            => new TokenizedStream<TInput, TState, TToken>(source, parser);
    }
}
