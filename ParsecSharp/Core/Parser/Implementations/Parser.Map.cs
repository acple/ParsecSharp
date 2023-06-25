using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Map<TToken, TIntermediate, T>(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => parser.Run(state, result => cont(result.Map(function)));
    }
}
