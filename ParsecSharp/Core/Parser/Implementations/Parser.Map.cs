using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Map<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, T> function) : IParser<TToken, T>
    {
        IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => cont(result.Map(function)));
    }

    internal sealed class MapWithExceptionHandling<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, T> function) : IParser<TToken, T>
    {
        IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => cont(this.TryMap(result, state)));

        private IResult<TToken, T> TryMap<TState>(IResult<TToken, TIntermediate> result, TState state)
            where TState : IParsecState<TToken, TState>
        {
            try
            {
                return result.Map(function);
            }
            catch (Exception exception)
            {
                return Result.Failure<TToken, TState, T>(exception, state);
            }
        }
    }
}
