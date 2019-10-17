using System;

namespace ParsecSharp
{
    public class Success<TToken, T> : Result<TToken, T>
    {
        public override T Value { get; }

        protected internal Success(T result, IParsecStateStream<TToken> state) : base(state)
        {
            this.Value = result;
        }

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => next(this.Value).Run(this.Rest, cont);

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => new Success<TToken, TResult>(function(this.Value), this.Rest);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
