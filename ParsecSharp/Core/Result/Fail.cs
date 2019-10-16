using System;

namespace ParsecSharp
{
    public abstract class Fail<TToken, T> : Result<TToken, T>
    {
        public sealed override T Value => throw this.Exception;

        public IParsecState<TToken> State => this.Rest;

        public virtual ParsecException Exception => new ParsecException(this.ToString());

        public abstract string Message { get; }

        protected Fail(IParsecStateStream<TToken> state) : base(state)
        { }

        public abstract Fail<TToken, TNext> Convert<TNext>();

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => cont(this.Convert<TNext>());

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => fail(this);

        public sealed override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => this.Convert<TResult>();

        public sealed override string ToString()
            => $"Parser Fail ({this.State.Position.ToString()}): {this.Message}";
    }
}
