using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Alternative<TToken, T>(Parser<TToken, T> first, Parser<TToken, T> second) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => first.Run(state, result => result.CaseOf(_ => second.Run(state, cont), cont));
    }
}
