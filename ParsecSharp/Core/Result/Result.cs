using System;

namespace ParsecSharp
{
    public abstract class Result<TToken, T>
    {
        public abstract T Value { get; }

        protected IParsecStateStream<TToken> Rest { get; }

        protected Result(IParsecStateStream<TToken> state)
        {
            this.Rest = state;
        }

        internal abstract Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont);

        public abstract TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success);

        public abstract Result<TToken, TResult> Map<TResult>(Func<T, TResult> function);

        internal Suspended Suspend()
            => new Suspended(this);

        public sealed override bool Equals(object? obj)
            => base.Equals(obj);

        public sealed override int GetHashCode()
            => base.GetHashCode();

        public readonly struct Suspended
        {
            private readonly Result<TToken, T> _result;

            internal Suspended(Result<TToken, T> result)
            {
                this._result = result;
            }

            public void Deconstruct(out Result<TToken, T> result, out IParsecStateStream<TToken> rest)
            {
                result = this._result;
                rest = this._result.Rest;
            }
        }
    }
}
