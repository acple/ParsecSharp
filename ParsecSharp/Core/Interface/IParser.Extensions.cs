using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;
using ParsecSharp.Internal;

namespace ParsecSharp;

public static class ParserExtensions
{
    extension<TToken, T>(IParser<TToken, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse<TState>(TState source)
            where TState : IParsecState<TToken, TState>
        {
            using (source.InnerResource)
                return parser.Run(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse(ISuspendedState<TToken> suspended)
        {
            using (suspended.InnerResource)
                return suspended.Continue(parser).Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISuspendedResult<TToken, T> ParsePartially<TState>(TState source)
            where TState : IParsecState<TToken, TState>
            => parser.Run(source).Suspend();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISuspendedResult<TToken, T> ParsePartially(ISuspendedState<TToken> suspended)
            => suspended.Continue(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IResult<TToken, T> Run<TState>(TState source)
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
