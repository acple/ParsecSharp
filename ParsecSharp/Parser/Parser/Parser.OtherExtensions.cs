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
                result => { failure(result); return default; },
                result => { success(result); return default; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action)
            => parser.Map(x => { action(x); return x; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action, Action<Failure<TToken, T>> failure)
            => new Do<TToken, T>(parser, failure, action);
    }
}
