using System;
using Parsec.Internal;

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

        private Parser<TToken, TNext> Next<TNext>(Func<T, Parser<TToken, TNext>> next)
        {
            try
            {
                return next(this.Value);
            }
            catch (Exception exception)
            {
                return new Exit<TToken, TNext>(exception);
            }
        }

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => this.Next(next).Run(this._state, cont);

        public override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
