using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Delay<TToken, T>(Func<IParser<TToken, T>> parser) : Parser<TToken, T>
    {
        private readonly Lazy<IParser<TToken, T>> _parser = new(parser, false);

        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => this._parser.Value.Run(state, cont);
    }
}
