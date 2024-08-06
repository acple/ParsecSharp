using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Next<TToken, TIntermediate, T>(Parser<TToken, TIntermediate> parser, Func<TIntermediate, Parser<TToken, T>> success, Func<Failure<TToken, TIntermediate>, Parser<TToken, T>> failure) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(
                result => failure(result).Run(state, cont),
                result => result.Next(success, cont)));
    }
}
