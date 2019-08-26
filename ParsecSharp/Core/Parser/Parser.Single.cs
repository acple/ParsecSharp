using System;

namespace ParsecSharp.Internal
{
    internal sealed class Single<TToken, T> : Parser<TToken, T>
    {
        private readonly Func<IParsecStateStream<TToken>, Result<TToken, T>> _function;

        internal Single(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
        {
            this._function = function;
        }

        internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => cont(this._function(state));
    }
}
