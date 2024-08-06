using System;

namespace ParsecSharp
{
    public abstract class Success<TToken, T>(T value) : Result<TToken, T>
    {
        public override T Value => value;

        protected abstract Result<TToken, TResult> RunNext<TNext, TResult>(Parser<TToken, TNext> parser, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont);

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => this.RunNext(next(value), cont);

        public sealed override TResult CaseOf<TResult>(Func<Failure<TToken, T>, TResult> failure, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => value?.ToString() ?? string.Empty;
    }
}
