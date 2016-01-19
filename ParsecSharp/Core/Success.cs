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

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> function)
            => function(this.Value).Run(this._state);

        public override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> _, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
