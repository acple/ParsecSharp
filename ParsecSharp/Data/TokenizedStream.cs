using System;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class TokenizedStream<TInput, TToken> : IParsecStateStream<TToken>
    {
        private readonly IDisposable disposable;

        private readonly LinearPosition _position;

        private readonly Lazy<IParsecStateStream<TToken>> _next;

        public TToken Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => this._next.Value;

        public TokenizedStream(IParsecStateStream<TInput> source, Parser<TInput, TToken> parser) : this(source, parser, LinearPosition.Initial)
        { }

        private TokenizedStream(IParsecStateStream<TInput> source, Parser<TInput, TToken> parser, LinearPosition position)
        {
            this.disposable = source;
            this._position = position;
            var (result, rest) = parser.ParsePartially(source);
            this.HasValue = result.CaseOf(_ => false, _ => true);
            this.Current = (this.HasValue) ? result.Value : default;
            this._next = new Lazy<IParsecStateStream<TToken>>(() => new TokenizedStream<TInput, TToken>(rest, parser, position.Next()), false);
        }

        public void Dispose()
            => this.disposable.Dispose();

        public bool Equals(IParsecState<TToken> other)
            => ReferenceEquals(this, other);

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
