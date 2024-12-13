using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Bind<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next)
            => new Bind<TToken, T, TResult>(parser, next);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Alternative<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second)
            => new Alternative<TToken, T>(first, second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Alternative<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, IParser<TToken, T>> resume)
            => new Resume<TToken, T>(parser, resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Map<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function)
            => new Map<TToken, T, TResult>(parser, function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> MapConst<TToken, TIgnore, T>(this IParser<TToken, TIgnore> parser, T result)
            => parser.Map(_ => result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> MapWithExceptionHandling<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function)
            => new MapWithExceptionHandling<TToken, T, TResult>(parser, function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, Func<IFailure<TToken, T>, IParser<TToken, TResult>> resume)
            => new Next<TToken, T, TResult>(parser, next, resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, TResult result)
            => parser.Next(next, Pure<TToken, TResult>(result).Const);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, Func<IFailure<TToken, T>, TResult> result)
            => parser.Next(next, failure => Pure<TToken, TResult>(result(failure)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, Func<IFailure<TToken, T>, IParser<TToken, TResult>> resume)
            => parser.Next(x => Pure<TToken, TResult>(function(x)), resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, TResult result)
            => new BimapConst<TToken, T, TResult>(parser, function, result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, Func<IFailure<TToken, T>, TResult> result)
            => new Bimap<TToken, T, TResult>(parser, function, result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Flatten<TToken, T>(this IParser<TToken, IParser<TToken, T>> parser)
            => parser.Bind(parser => parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Guard<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Guard(predicate, x => $"A value '{x?.ToString() ?? "<null>"}' does not satisfy condition");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Guard<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate, Func<T, string> message)
            => parser.Bind(x => predicate(x) ? Pure<TToken, T>(x) : Fail<TToken, T>(message(x)));
    }
}
