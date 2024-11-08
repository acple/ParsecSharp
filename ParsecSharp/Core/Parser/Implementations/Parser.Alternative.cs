using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Alternative<TToken, T>(IParser<TToken, T> first, IParser<TToken, T> second) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => first.Run(state, result => result.CaseOf(_ => second.Run(state, cont), cont));
    }
}
