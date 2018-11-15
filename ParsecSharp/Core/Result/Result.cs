using System;

namespace Parsec
{
    public abstract class Result<TToken, T> : ISuspendedResult<TToken, T>
    {
        public abstract T Value { get; }

        protected IParsecStateStream<TToken> Rest { get; }

        public Result(IParsecStateStream<TToken> state)
        {
            this.Rest = state;
        }

        internal abstract Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont);

        public abstract TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success);

        public abstract override string ToString();

        public static implicit operator T(Result<TToken, T> result)
            => result.Value;

        void ISuspendedResult<TToken, T>.Deconstruct(out Result<TToken, T> result, out IParsecStateStream<TToken> rest)
        {
            result = this;
            rest = this.Rest;
        }
    }
}
