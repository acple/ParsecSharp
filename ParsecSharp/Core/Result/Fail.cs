using System;

namespace Parsec
{
    public abstract class Fail<TToken, T> : Result<TToken, T>
    {
        public sealed override T Value => throw this.Exception;

        public IParsecState<TToken> State => this.Rest;

        public virtual ParsecException Exception => new ParsecException(this.ToString());

        protected Fail(IParsecStateStream<TToken> state) : base(state)
        { }

        protected abstract Fail<TToken, TNext> Convert<TNext>();

        internal override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => cont(this.Convert<TNext>());

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => fail(this);

        protected abstract string ToStringInternal();

        public sealed override string ToString()
            => $"Parser Fail ({this.State.Position.ToString()}): {this.ToStringInternal()}";
    }
}
