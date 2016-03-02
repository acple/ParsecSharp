using System;

namespace Parsec
{
    public abstract class Fail<TToken, T> : Result<TToken, T>
    {
        public IParsecState<TToken> State { get; }

        public abstract ParsecException<TToken> Exception { get; }

        public sealed override T Value { get { throw this.Exception; } }

        protected Fail(IParsecState<TToken> state)
        {
            this.State = state;
        }

        protected abstract Result<TToken, TResult> Next<TResult>();

        internal sealed override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> _)
            => this.Next<TResult>();

        public override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> _)
            => fail(this);

        protected abstract string ToStringInternal();

        public sealed override string ToString()
            => $"Parser Fail ({ this.State.Position }): { this.ToStringInternal() }";
    }
}
