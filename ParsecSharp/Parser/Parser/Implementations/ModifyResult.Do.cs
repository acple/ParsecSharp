using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Do<TToken, T> : ModifyResult<TToken, T, T>
    {
        private readonly Action<Failure<TToken, T>> _fail;

        private readonly Action<T> _succeed;

        public Do(Parser<TToken, T> parser, Action<Failure<TToken, T>> fail, Action<T> succeed) : base(parser)
        {
            this._fail = fail;
            this._succeed = succeed;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
        {
            this._fail(failure);
            return failure;
        }

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
        {
            this._succeed(success.Value);
            return success;
        }
    }
}
