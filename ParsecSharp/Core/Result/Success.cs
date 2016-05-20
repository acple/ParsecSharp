using System;

namespace Parsec
{
    public class Success<TToken, T> : Result<TToken, T>
    {
        private readonly IParsecStateStream<TToken> _state;

        public override T Value { get; }

        internal Success(T result, IParsecStateStream<TToken> state)
        {
            this.Value = result;
            this._state = state;
        }

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => next(this.Value).Run(this._state, cont);

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont, Func<Result<TToken, TResult>> resume)
            => next(this.Value).Run(this._state, cont, resume);

        public override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
