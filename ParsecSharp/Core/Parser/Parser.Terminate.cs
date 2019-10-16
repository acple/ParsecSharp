using System;

namespace ParsecSharp.Internal
{
    internal sealed class Terminate<TToken, T> : Parser<TToken, T>
    {
        private readonly Func<IParsecStateStream<TToken>, Fail<TToken, T>> _fail;

        internal Terminate(Func<IParsecState<TToken>, string> message) : this(state => new FailWithMessage<TToken, T>(message(state), state))
        { }

        internal Terminate(Exception exception) : this(state => new FailWithException<TToken, T>(exception, state))
        { }

        private Terminate(Func<IParsecStateStream<TToken>, Fail<TToken, T>> fail)
        {
            this._fail = fail;
        }

        internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._fail(state).Convert<TResult>();
    }
}
