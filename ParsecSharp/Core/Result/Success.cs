using System;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public class Success<TToken, T> : Result<TToken, T>
    {
        public override T Value { get; }

        protected internal Success(T result, IParsecStateStream<TToken> state) : base(state)
        {
            this.Value = result;
        }

        private Parser<TToken, TNext> TryNext<TNext>(Func<T, Parser<TToken, TNext>> next)
        {
            try
            {
                return next(this.Value);
            }
            catch (Exception exception)
            {
                return new Abort<TToken, TNext>(exception);
            }
        }

        internal sealed override Result<TToken, TResult> Next<TNext, TResult>(Func<T, Parser<TToken, TNext>> next, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => this.TryNext(next).Run(this.Rest, cont);

        public sealed override TResult CaseOf<TResult>(Func<Fail<TToken, T>, TResult> fail, Func<Success<TToken, T>, TResult> success)
            => success(this);

        public override string ToString()
            => this.Value?.ToString() ?? string.Empty;
    }
}
