using System;

namespace ParsecSharp
{
    public abstract class Failure<TToken, T> : Result<TToken, T>
    {
        public sealed override T Value => throw this.Exception;

        public abstract IParsecState<TToken> State { get; }

        public virtual ParsecException Exception => new(this.ToString());

        public abstract string Message { get; }

        public abstract Failure<TToken, TNext> Convert<TNext>();

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => cont(this.Convert<TNext>());

        public sealed override TResult CaseOf<TResult>(Func<Failure<TToken, T>, TResult> failure, Func<Success<TToken, T>, TResult> success)
            => failure(this);

        public sealed override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => this.Convert<TResult>();

        public sealed override string ToString()
            => $"Parser Failure ({this.State.Position.ToString()}): {this.Message}";
    }
}
