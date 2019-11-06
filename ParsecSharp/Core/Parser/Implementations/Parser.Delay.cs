using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Delay<TToken, T> : Parser<TToken, T>
    {
        private readonly Lazy<Parser<TToken, T>> _parser;

        public Delay(Func<Parser<TToken, T>> parser)
        {
            this._parser = new Lazy<Parser<TToken, T>>(parser, false);
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Value.Run(state, cont);
    }
}
