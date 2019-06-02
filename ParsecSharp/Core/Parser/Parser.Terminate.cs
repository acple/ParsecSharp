using System;

namespace ParsecSharp.Internal
{
    internal sealed class Terminate<TToken, T> : Parser<TToken, T>
    {
        private readonly Func<IParsecState<TToken>, string> _message;

        internal Terminate(Func<IParsecState<TToken>, string> message)
        {
            this._message = message;
        }

        protected internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => new FailWithMessage<TToken, TResult>(this._message(state), state);
    }
}
