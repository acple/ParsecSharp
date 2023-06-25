using System;

namespace ParsecSharp.Internal.Results
{
    internal sealed class Success<TToken, TState, T>(T result, TState state) : Success<TToken, T>(result)
        where TState : IParsecState<TToken, TState>
    {
        protected sealed override Result<TToken, TResult> RunNext<TNext, TResult>(Parser<TToken, TNext> parser, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
            => parser.Run(state, cont);

        protected internal sealed override SuspendedResult<TToken, T> Suspend()
            => SuspendedResult.Create(this, state);

        public sealed override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
            => new Success<TToken, TState, TResult>(function(this.Value), state);
    }
}
