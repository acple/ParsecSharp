using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Try<TToken, T> : ModifyResult<TToken, T, T>
    {
        private readonly Func<Failure<TToken, T>, T> _resume;

        public Try(Parser<TToken, T> parser, Func<Failure<TToken, T>, T> resume) : base(parser)
        {
            this._resume = resume;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
            => Result.Success<TToken, TState, T>(this._resume(failure), state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
            => success;
    }
}
