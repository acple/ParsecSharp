using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static class ParserExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<TToken, T> Parse<TToken, T, TState>(this IParser<TToken, T> parser, TState source)
            where TState : IParsecState<TToken, TState>
        {
            using (source.InnerResource)
                return parser.Run(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IResult<TToken, T> Parse<TToken, T>(this IParser<TToken, T> parser, ISuspendedState<TToken> suspended)
        {
            using (suspended.InnerResource)
                return suspended.Continue(parser).Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISuspendedResult<TToken, T> ParsePartially<TToken, T, TState>(this IParser<TToken, T> parser, TState source)
            where TState : IParsecState<TToken, TState>
            => parser.Run(source).Suspend();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISuspendedResult<TToken, T> ParsePartially<TToken, T>(this IParser<TToken, T> parser, ISuspendedState<TToken> suspended)
            => suspended.Continue(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IResult<TToken, T> Run<TToken, T, TState>(this IParser<TToken, T> parser, TState source)
            where TState : IParsecState<TToken, TState>
        {
            try
            {
                return parser.Run(source, result => result);
            }
            catch (Exception exception)
            {
                return Result.Failure<TToken, EmptyStream<TToken>, T>(exception, EmptyStream<TToken>.Instance);
            }
        }
    }
}
