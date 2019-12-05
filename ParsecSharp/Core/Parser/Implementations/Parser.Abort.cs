using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Abort<TToken, T> : Parser<TToken, T>
    {
        private readonly Exception _exception;

        public Abort(Exception exception)
        {
            this._exception = exception;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(this._exception, state);
    }
}
