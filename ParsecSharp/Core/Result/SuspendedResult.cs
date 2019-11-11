using System;
using System.Runtime.InteropServices;

namespace ParsecSharp
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct SuspendedResult<TToken, T> : ISuspendedState<TToken>
    {
        public Result<TToken, T> Result { get; }

        public ISuspendedState<TToken> Rest { get; }

        IDisposable? ISuspendedState<TToken>.InnerResource => this.Rest?.InnerResource;

        private SuspendedResult(Result<TToken, T> result, ISuspendedState<TToken> rest)
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
            => this.Rest?.Dispose();

        public static SuspendedResult<TToken, T> Create<TState>(Result<TToken, T> result, TState state)
            where TState : IParsecState<TToken, TState>
            => new SuspendedResult<TToken, T>(result, new StateBox<TState>(state));

        private sealed class StateBox<TState> : ISuspendedState<TToken>
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
