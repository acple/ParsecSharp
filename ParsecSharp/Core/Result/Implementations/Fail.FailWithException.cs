using System;

namespace ParsecSharp.Internal.Results
{
    internal sealed class FailWithException<TToken, TState, T> : Fail<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _state;

        private readonly Exception _exception;

        public sealed override IParsecState<TToken> State => this._state.GetState();

        public sealed override ParsecException Exception => new ParsecException(this.ToString(), this._exception);

        public sealed override string Message => $"Exception '{this._exception.GetType().Name}' occurred: {this._exception.ToString()}";

        public FailWithException(Exception exception, TState state)
        {
            this._state = state;
            this._exception = exception;
        }

        public sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult<TToken, T>.Create(this, this._state);

        public sealed override Fail<TToken, TNext> Convert<TNext>()
            => new FailWithException<TToken, TState, TNext>(this._exception, this._state);
    }
}
