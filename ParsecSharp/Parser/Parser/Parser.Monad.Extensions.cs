using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Bind<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next)
            => new Bind<TToken, T, TResult>(parser, next);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Alternative<TToken, T>(this Parser<TToken, T> first, Parser<TToken, T> second)
            => new Alternative<TToken, T>(first, second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Alternative<TToken, T>(this Parser<TToken, T> parser, Func<Fail<TToken, T>, Parser<TToken, T>> resume)
            => new Resume<TToken, T>(parser, resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Map<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function)
            => new Map<TToken, T, TResult>(parser, function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next, Func<Fail<TToken, T>, Parser<TToken, TResult>> resume)
            => new Next<TToken, T, TResult>(parser, next, resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next, TResult result)
            => parser.Next(next, _ => Pure<TToken, TResult>(result));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next, Func<TResult> result)
            => parser.Next(next, _ => Pure<TToken, TResult>(result));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function, Func<Fail<TToken, T>, Parser<TToken, TResult>> resume)
            => parser.Next(x => Pure<TToken, TResult>(function(x)), resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function, TResult result)
            => parser.ModifyResult((state, _) => Result.Success(result, state), (_, success) => success.Map(function));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function, Func<TResult> result)
            => parser.ModifyResult((state, _) => Result.Success(result(), state), (_, success) => success.Map(function));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Guard<TToken, T>(this Parser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Guard(predicate, x => $"A value '{x?.ToString() ?? "<null>"}' does not satisfy condition");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Guard<TToken, T>(this Parser<TToken, T> parser, Func<T, bool> predicate, Func<T, string> message)
            => parser.Bind(x => (predicate(x)) ? Pure<TToken, T>(x) : Fail<TToken, T>($"At {nameof(Guard)} -> {message(x)}"));
    }
}
