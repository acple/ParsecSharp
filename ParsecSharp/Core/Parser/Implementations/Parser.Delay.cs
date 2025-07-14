using System;
using System.Threading;

namespace ParsecSharp.Internal.Parsers;

internal sealed class Delay<TToken, T>(Func<IParser<TToken, T>> parser) : IParser<TToken, T>
{
    private readonly Lazy<IParser<TToken, T>> _parser = new(parser, LazyThreadSafetyMode.PublicationOnly);

    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => this._parser.Value.Run(state, cont);
}
