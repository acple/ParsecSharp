using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Resume<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, IParser<TToken, T>> resume) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(failure => resume(failure).Run(state, cont), cont));
    }
}
