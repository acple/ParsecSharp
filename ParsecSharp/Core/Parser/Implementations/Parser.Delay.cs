using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Delay<TToken, T>(Func<Parser<TToken, T>> parser) : Parser<TToken, T>
    {
        private readonly Lazy<Parser<TToken, T>> _parser = new(parser, false);

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Value.Run(state, cont);
    }
}
