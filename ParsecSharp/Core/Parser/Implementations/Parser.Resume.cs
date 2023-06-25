using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Resume<TToken, T>(Parser<TToken, T> parser, Func<Failure<TToken, T>, Parser<TToken, T>> resume) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(failure => resume(failure).Run(state, cont), cont));
    }
}
