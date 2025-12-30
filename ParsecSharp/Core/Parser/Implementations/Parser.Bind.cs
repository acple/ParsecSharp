using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Bind<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, IParser<TToken, T>> next) : IParser<TToken, T>
{
    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => parser.Run(state, result => result.CaseOf(failure => cont(failure.Coerce<T>()), success => success.Next(next, cont)));
}
