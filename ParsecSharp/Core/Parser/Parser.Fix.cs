using System;

namespace ParsecSharp.Internal
{
    internal sealed class Fix<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _parser;

        internal Fix(Func<Parser<TToken, T>, Parser<TToken, T>> function)
        {
            this._parser = function(this);
        }

        internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Run(state, cont);
    }
}
