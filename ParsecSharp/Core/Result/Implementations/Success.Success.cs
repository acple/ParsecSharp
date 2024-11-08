using System;

namespace ParsecSharp.Internal.Results
{
    internal sealed class Success<TToken, TState, T>(T value, TState state) : Success<TToken, T>(value)
        where TState : IParsecState<TToken, TState>
    {
        public sealed override IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => new Success<TToken, TState, TResult>(function(this.Value), state);

        public sealed override IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont)
            => next(this.Value).Run(state, cont);

        public sealed override ISuspendedResult<TToken, T> Suspend()
            => SuspendedResult.Create(this, state);
    }
}
