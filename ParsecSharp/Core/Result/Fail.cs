using System;

namespace Parsec
{
    public abstract class Fail<TToken, T> : Result<TToken, T>
    {
        public IParsecState<TToken> State { get; }

        public virtual ParsecException<TToken> Exception => new ParsecException<TToken>(this.ToString(), this.State);

        public sealed override T Value { get { throw this.Exception; } }

        protected Fail(IParsecState<TToken> state)
        {
            this.State = state;
        }

        protected abstract Fail<TToken, TResult> Next<TResult>();

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => this.Next<TResult>();

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont, Func<Result<TToken, TResult>> resume)
            => resume();

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => fail(this);

        protected abstract string ToStringInternal();

        public sealed override string ToString()
            => $"Parser Fail ({ this.State.Position }): { this.ToStringInternal() }";
    }
}
