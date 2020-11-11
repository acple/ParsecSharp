using System;

namespace ParsecSharp
{
    public abstract class Result<TToken, T>
    {
        public abstract T Value { get; }

        private protected Result()
        { }

        internal abstract Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont);

        public abstract TResult CaseOf<TResult>(Func<Failure<TToken, T>, TResult> failure, Func<Success<TToken, T>, TResult> success);

        public abstract Result<TToken, TResult> Map<TResult>(Func<T, TResult> function);

        protected internal abstract SuspendedResult<TToken, T> Suspend();

        public sealed override bool Equals(object? obj)
            => base.Equals(obj);

        public sealed override int GetHashCode()
            => base.GetHashCode();
    }
}
