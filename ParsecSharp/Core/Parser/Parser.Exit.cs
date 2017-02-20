using System;

namespace Parsec.Internal
{
    internal class Exit<TToken, T> : Parser<TToken, T>
    {
        private readonly Exception _exception;

        internal Exit(Exception exception)
        {
            this._exception = exception;
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => new FailWithException<TToken, TResult>(this._exception, state);
    }
}
