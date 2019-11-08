using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Bimap<TToken, TIntermediate, T> : ModifyResult<TToken, TIntermediate, T>
    {
        private readonly Func<TIntermediate, T> _function;

        private readonly Func<Failure<TToken, TIntermediate>, T> _result;

        public Bimap(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, Func<Failure<TToken, TIntermediate>, T> result) : base(parser)
        {
            this._function = function;
            this._result = result;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIntermediate> failure)
            => Result.Success<TToken, TState, T>(this._result(failure), state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIntermediate> success)
            => success.Map(this._function);
    }

    internal sealed class BimapConst<TToken, TIntermediate, T> : ModifyResult<TToken, TIntermediate, T>
    {
        private readonly Func<TIntermediate, T> _function;

        private readonly T _result;

        public BimapConst(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function, T result) : base(parser)
        {
            this._function = function;
            this._result = result;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIntermediate> failure)
            => Result.Success<TToken, TState, T>(this._result, state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIntermediate> success)
            => success.Map(this._function);
    }
}
