using System;

namespace ParsecSharp.Internal
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
        {
            var _second = this._second;
            return this._first.Run(state, result => result.CaseOf(_ => _second.Run(state, cont), cont));
        }
    }
}
