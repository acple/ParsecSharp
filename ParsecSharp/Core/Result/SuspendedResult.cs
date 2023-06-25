using System;

namespace ParsecSharp
{
    public readonly struct SuspendedResult<TToken, T>(Result<TToken, T> result, ISuspendedState<TToken> rest) : ISuspendedState<TToken>, IEquatable<SuspendedResult<TToken, T>>
    {
        public Result<TToken, T> Result => result;

        public ISuspendedState<TToken> Rest => rest;

        IDisposable? ISuspendedState<TToken>.InnerResource => rest.InnerResource;

        public void Deconstruct(out Result<TToken, T> result, out ISuspendedState<TToken> rest)
        {
            result = this.Result;
            rest = this.Rest;
        }

        SuspendedResult<TToken, TResult> ISuspendedState<TToken>.Continue<TResult>(Parser<TToken, TResult> parser)
            => rest.Continue(parser);

        public void Dispose()
            => rest.Dispose();

        public bool Equals(SuspendedResult<TToken, T> other)
            => result == other.Result && rest == other.Rest;

        public override bool Equals(object? obj)
            => obj is SuspendedResult<TToken, T> other && result == other.Result && rest == other.Rest;

        public override int GetHashCode()
            => result.GetHashCode() ^ rest.GetHashCode();

        public static bool operator ==(SuspendedResult<TToken, T> left, SuspendedResult<TToken, T> right)
            => left.Result == right.Result && left.Rest == right.Rest;

        public static bool operator !=(SuspendedResult<TToken, T> left, SuspendedResult<TToken, T> right)
            => !(left == right);
    }
}

namespace ParsecSharp.Internal
{
    public static class SuspendedResult
    {
        public static SuspendedResult<TToken, T> Create<TToken, TState, T>(Result<TToken, T> result, TState state)
            where TState : IParsecState<TToken, TState>
            => new(result, new StateBox<TToken, TState>(state));

        private sealed class StateBox<TToken, TState>(TState state) : ISuspendedState<TToken>
            where TState : IParsecState<TToken, TState>
        {
            public IDisposable? InnerResource => state.InnerResource;

            public SuspendedResult<TToken, TResult> Continue<TResult>(Parser<TToken, TResult> parser)
                => parser.ParsePartially(state);

            public void Dispose()
                => state.Dispose();
        }
    }
}
