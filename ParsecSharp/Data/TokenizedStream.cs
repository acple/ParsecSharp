using System;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class TokenizedStream<TInput, TState, TToken> : IParsecState<TToken, TokenizedStream<TInput, TState, TToken>>
        where TState : IParsecState<TInput, TState>
    {
        private readonly LinearPosition _position;

        private readonly Lazy<TokenizedStream<TInput, TState, TToken>> _next;

        public TToken Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IDisposable? InnerResource { get; }

        public TokenizedStream<TInput, TState, TToken> Next => this._next.Value;

        public TokenizedStream(TState source, Parser<TInput, TToken> parser) : this(source.InnerResource, parser.ParsePartially(source), parser, LinearPosition.Initial)
        { }

        private TokenizedStream(IDisposable? resource, SuspendedResult<TInput, TToken> state, Parser<TInput, TToken> parser, LinearPosition position)
        {
            this.InnerResource = resource;
            this._position = position;
            var (result, rest) = state;
            this.HasValue = result.CaseOf(_ => false, _ => true);
            this.Current = (this.HasValue) ? result.Value : default!;
            this._next = new Lazy<TokenizedStream<TInput, TState, TToken>>(() => new TokenizedStream<TInput, TState, TToken>(rest.InnerResource, parser.ParsePartially(rest), parser, position.Next()), false);
        }

        public IParsecState<TToken> GetState()
            => this;

        public void Dispose()
            => this.InnerResource?.Dispose();

        public bool Equals(TokenizedStream<TInput, TState, TToken> other)
            => ReferenceEquals(this, other);

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
