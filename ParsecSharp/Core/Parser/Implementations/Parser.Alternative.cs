using System;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Alternative<TToken, T>(IParser<TToken, T> first, IParser<TToken, T> second) : IParser<TToken, T>
{
    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => first.Run(state, result => result.CaseOf(_ => second.Run(state, cont), cont));
}
