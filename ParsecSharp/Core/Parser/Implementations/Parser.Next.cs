using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Next<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, IParser<TToken, T>> success, Func<IFailure<TToken, TIntermediate>, IParser<TToken, T>> failure) : IParser<TToken, T>
    {
        IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(
                result => failure(result).Run(state, cont),
                result => result.Next(success, cont)));
    }
}
