using System;

namespace ParsecSharp
{
    public abstract class Success<TToken, T> : Result<TToken, T>
    {
        public override T Value { get; }

        protected Success(T result)
        {
            this.Value = result;
        }

        protected abstract Result<TToken, TResult> RunNext<TNext, TResult>(Parser<TToken, TNext> parser, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont);

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => this.RunNext(next(this.Value), cont);

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
