using System;

namespace Parsec
{
    public abstract class Fail<TToken, T> : Result<TToken, T>
    {
        protected IParsecStateStream<TToken> State { get; }

        public abstract ParsecException Exception { get; }

        public sealed override T Value { get { throw this.Exception; } }

        public IPosition Position => this.State.Position;

        internal Fail(IParsecStateStream<TToken> state)
        {
            this.State = state;
        }

        public override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> _)
            => fail(this);

        protected abstract string ToStringInternal();

        public sealed override string ToString()
            => $"Parser Fail ({ this.Position }): { this.ToStringInternal() }";
    }
}
