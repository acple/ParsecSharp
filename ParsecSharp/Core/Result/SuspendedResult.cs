using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    public static class SuspendedResult
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISuspendedResult<TToken, T> Create<TToken, TState, T>(IResult<TToken, T> result, TState state)
            where TState : IParsecState<TToken, TState>
            => new SuspendedResult<TToken, T>(result, new StateBox<TToken, TState>(state));

        private sealed class StateBox<TToken, TState>(TState state) : ISuspendedState<TToken>
            where TState : IParsecState<TToken, TState>
        {
            public IDisposable? InnerResource => state.InnerResource;

            public ISuspendedResult<TToken, TResult> Continue<TResult>(IParser<TToken, TResult> parser)
                => parser.ParsePartially(state);

            public void Dispose()
                => state.Dispose();
        }
    }

    public sealed class SuspendedResult<TToken, T>(IResult<TToken, T> result, ISuspendedState<TToken> rest) : ISuspendedResult<TToken, T>, IEquatable<SuspendedResult<TToken, T>>
    {
        public IResult<TToken, T> Result { get; } = result;

        public ISuspendedState<TToken> Rest { get; } = rest;

        public void Deconstruct(out IResult<TToken, T> result, out ISuspendedState<TToken> rest)
        {
            result = this.Result;
            rest = this.Rest;
        }

        public void Dispose()
            => this.Rest.Dispose();

        public bool Equals(SuspendedResult<TToken, T>? other)
            => other is not null && this.Result == other.Result && this.Rest == other.Rest;

        public sealed override bool Equals(object? obj)
            => obj is SuspendedResult<TToken, T> other && this.Result == other.Result && this.Rest == other.Rest;

        public sealed override int GetHashCode()
            => this.Result.GetHashCode() ^ this.Rest.GetHashCode();

        public static bool operator ==(SuspendedResult<TToken, T> left, SuspendedResult<TToken, T> right)
            => left.Result == right.Result && left.Rest == right.Rest;

        public static bool operator !=(SuspendedResult<TToken, T> left, SuspendedResult<TToken, T> right)
            => !(left == right);
    }
}
