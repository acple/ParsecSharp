using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Do<TToken, T> : ModifyResult<TToken, T, T>
    {
        private readonly Action<Fail<TToken, T>> _fail;

        private readonly Action<T> _success;

        public Do(Parser<TToken, T> parser, Action<Fail<TToken, T>> fail, Action<T> success) : base(parser)
        {
            this._fail = fail;
            this._success = success;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Fail<TToken, T> fail)
        {
            this._fail(fail);
            return fail;
        }

        protected sealed override Result<TToken, T> Success<TState>(TState state, Success<TToken, T> success)
        {
            this._success(success.Value);
            return success;
        }
    }
}
