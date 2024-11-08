using System;

namespace ParsecSharp.Internal
{
    public abstract class Failure<TToken, T> : IFailure<TToken, T>
    {
        public T Value => throw this.Exception;

        public abstract IParsecState<TToken> State { get; }

        public virtual ParsecException Exception => new(this.ToString());

        public abstract string Message { get; }

        public abstract IResult<TToken, TResult> Convert<TResult>();

        public abstract ISuspendedResult<TToken, T> Suspend();

        public TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success)
            => failure(this);

        public IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => this.Convert<TResult>();

        public sealed override string ToString()
            => $"Parser Failure ({this.State.Position.ToString()}): {this.Message}";
    }
}
