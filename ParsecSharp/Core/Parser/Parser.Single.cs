using System;

namespace Parsec.Internal
{
    internal class Single<TToken, T> : Parser<TToken, T>
    {
        private readonly Func<IParsecStateStream<TToken>, Result<TToken, T>> _function;

        internal Single(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
        {
            this._function = function;
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => cont(this._function(state));

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont, Func<Result<TToken, TResult>> resume)
            => this._function(state).CaseOf(fail => resume(), cont);
    }
}
