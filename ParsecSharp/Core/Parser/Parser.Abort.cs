using System;

namespace ParsecSharp.Internal
{
    internal sealed class Abort<TToken, T> : Parser<TToken, T>
    {
        private readonly Exception _exception;

        internal Abort(Exception exception)
        {
            this._exception = exception;
        }

        protected internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => new FailWithException<TToken, TResult>(this._exception, state);
    }
}
