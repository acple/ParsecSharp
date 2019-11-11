using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Alternative<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _first;

        private readonly Parser<TToken, T> _second;

        public Alternative(Parser<TToken, T> first, Parser<TToken, T> second)
        {
            this._first = first;
            this._second = second;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _second = this._second;
            return this._first.Run(state, result => result.CaseOf(_ => _second.Run(state, cont), cont));
        }
    }
}
