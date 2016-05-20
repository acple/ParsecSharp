using System;

namespace Parsec.Internal
{
    internal class Alternative<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _first;

        private readonly Parser<TToken, T> _second;

        internal Alternative(Parser<TToken, T> first, Parser<TToken, T> second)
        {
            this._first = first;
            this._second = second;
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._first.Run(state, cont, () => this._second.Run(state, cont));

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont, Func<Result<TToken, TResult>> resume)
            => this._first.Run(state, cont, () => this._second.Run(state, cont, resume));
    }
}
