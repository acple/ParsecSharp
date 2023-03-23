using System;

namespace ParsecSharp
{
    public readonly struct SuspendedResult<TToken, T> : ISuspendedState<TToken>, IEquatable<SuspendedResult<TToken, T>>
    {
        public Result<TToken, T> Result { get; }

        public ISuspendedState<TToken> Rest { get; }

        IDisposable? ISuspendedState<TToken>.InnerResource => this.Rest.InnerResource;

        public SuspendedResult(Result<TToken, T> result, ISuspendedState<TToken> rest)
        {
            this.Result = result;
            this.Rest = rest;
        }

        public void Deconstruct(out Result<TToken, T> result, out ISuspendedState<TToken> rest)
        {
            result = this.Result;
            rest = this.Rest;
        }

        SuspendedResult<TToken, TResult> ISuspendedState<TToken>.Continue<TResult>(Parser<TToken, TResult> parser)
            => this.Rest.Continue(parser);

        public void Dispose()
            => this.Rest.Dispose();

        public bool Equals(SuspendedResult<TToken, T> other)
            => this.Result == other.Result && this.Rest == other.Rest;

        public override bool Equals(object? obj)
            => obj is SuspendedResult<TToken, T> other && this.Result == other.Result && this.Rest == other.Rest;

        public override int GetHashCode()
            => this.Result.GetHashCode() ^ this.Rest.GetHashCode();

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

        private sealed class StateBox<TToken, TState> : ISuspendedState<TToken>
            where TState : IParsecState<TToken, TState>
        {
            private readonly TState _state;

            public IDisposable? InnerResource => this._state.InnerResource;

            public StateBox(TState state)
            {
                this._state = state;
            }

            public SuspendedResult<TToken, TResult> Continue<TResult>(Parser<TToken, TResult> parser)
                => parser.ParsePartially(this._state);

            public void Dispose()
                => this._state.Dispose();
        }
    }
}
