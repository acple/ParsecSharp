using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Terminate<TToken, T> : Parser<TToken, T>
    {
        private readonly Func<IParsecState<TToken>, string> _message;

        public Terminate(Func<IParsecState<TToken>, string> message)
        {
            this._message = message;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(this._message(state), state);
    }
}
