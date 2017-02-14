using System;

namespace Parsec.Internal
{
    internal class Fix<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _parser;

        internal Fix(Func<Parser<TToken, T>, Parser<TToken, T>> parser)
        {
            this._parser = parser(this);
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Run(state, cont);
    }
}
