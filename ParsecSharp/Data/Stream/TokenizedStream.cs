using System;

namespace ParsecSharp.Internal
{
    public sealed class TokenizedStream<TInput, TState, TToken, TPosition> : IParsecState<TToken, TokenizedStream<TInput, TState, TToken, TPosition>>
        where TState : IParsecState<TInput, TState>
        where TPosition : IPosition<TToken, TPosition>
    {
        private readonly TPosition _position;

        private readonly Lazy<TokenizedStream<TInput, TState, TToken, TPosition>> _next;

        public TToken Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IDisposable? InnerResource { get; }

        public TokenizedStream<TInput, TState, TToken, TPosition> Next => this._next.Value;

        public TokenizedStream(TState source, Parser<TInput, TToken> parser, TPosition position) : this(source.InnerResource, parser.ParsePartially(source), parser, position)
        { }

        private TokenizedStream(IDisposable? resource, SuspendedResult<TInput, TToken> state, Parser<TInput, TToken> parser, TPosition position)
        {
            this.InnerResource = resource;
            this._position = position;
            var (result, rest) = state;
            this.HasValue = result.CaseOf(_ => false, _ => true);
            var current = this.Current = this.HasValue ? result.Value : default!;
            this._next = new(() => new(rest.InnerResource, parser.ParsePartially(rest), parser, position.Next(current)), false);
        }

        public void Dispose()
            => this.InnerResource?.Dispose();

        public bool Equals(TokenizedStream<TInput, TState, TToken, TPosition>? other)
            => ReferenceEquals(this, other);

        public sealed override string ToString()
            => this.HasValue
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
