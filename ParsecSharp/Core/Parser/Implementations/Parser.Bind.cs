using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Bind<TToken, TIntermediate, T>(Parser<TToken, TIntermediate> parser, Func<TIntermediate, Parser<TToken, T>> next) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => parser.Run(state, result => result.Next(next, cont));
    }
}
