using System;

namespace Parsec.Internal
{
    internal class Delay<TToken, T> : Parser<TToken, T>
    {
        private readonly Lazy<Parser<TToken, T>> _parser;

        internal Delay(Func<Parser<TToken, T>> parser)
        {
            this._parser = new Lazy<Parser<TToken, T>>(parser, false);
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Value.Run(state, cont);
    }
}
