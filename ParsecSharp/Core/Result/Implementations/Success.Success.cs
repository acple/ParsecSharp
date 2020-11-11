using System;

namespace ParsecSharp.Internal.Results
{
    internal sealed class Success<TToken, TState, T> : Success<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _state;

        public Success(T result, TState state) : base(result)
        {
            this._state = state;
        }

        protected sealed override Result<TToken, TResult> RunNext<TNext, TResult>(Parser<TToken, TNext> parser, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => parser.Run(this._state, cont);

        protected internal sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult<TToken, T>.Create(this, this._state);

        public sealed override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => new Success<TToken, TState, TResult>(function(this.Value), this._state);
    }
}
