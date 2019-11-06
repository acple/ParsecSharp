using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Try<TToken, T> : ModifyResult<TToken, T, T>
    {
        private readonly Func<Fail<TToken, T>, T> _resume;

        public Try(Parser<TToken, T> parser, Func<Fail<TToken, T>, T> resume) : base(parser)
        {
            this._resume = resume;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Fail<TToken, T> fail)
            => Result.Success<TToken, TState, T>(this._resume(fail), state);

        protected sealed override Result<TToken, T> Success<TState>(TState state, Success<TToken, T> success)
            => success;
    }
}
