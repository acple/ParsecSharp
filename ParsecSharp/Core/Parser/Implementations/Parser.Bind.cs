using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Bind<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, IParser<TToken, T>> next) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(failure => cont(failure.Convert<T>()), success => success.Next(next, cont)));
    }
}
