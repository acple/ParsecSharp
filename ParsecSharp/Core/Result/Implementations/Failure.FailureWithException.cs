using System;

namespace ParsecSharp.Internal.Results
{
    internal sealed class FailureWithException<TToken, TState, T>(Exception exception, TState state) : Failure<TToken, T>
        where TState : IParsecState<TToken, TState>
    {
        public sealed override IParsecState<TToken> State => state;

        public sealed override ParsecException Exception => new(this.ToString(), exception);

        public sealed override string Message => $"Exception '{exception.GetType().Name}' occurred: {exception.ToString()}";

        public sealed override ISuspendedResult<TToken, T> Suspend()
            => SuspendedResult.Create(this, state);

        public sealed override IResult<TToken, TNext> Convert<TNext>()
            => new FailureWithException<TToken, TState, TNext>(exception, state);
    }
}
