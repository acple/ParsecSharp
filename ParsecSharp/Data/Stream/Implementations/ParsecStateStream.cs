using System;

namespace ParsecSharp.Data
{
    public sealed class ParsecStateStream<TToken, TPosition> : IParsecState<TToken, ParsecStateStream<TToken, TPosition>>
        where TPosition : IPosition<TToken, TPosition>
    {
        private readonly TPosition _position;

        private readonly Lazy<ParsecStateStream<TToken, TPosition>> _next;

        public TToken Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IDisposable? InnerResource { get; }

        public ParsecStateStream<TToken, TPosition> Next => this._next.Value;

        public ParsecStateStream(TToken value, TPosition position, IDisposable? resource, Func<ParsecStateStream<TToken, TPosition>> next)
        {
            this.Current = value;
            this.HasValue = true;
            this._position = position;
            this.InnerResource = resource;
            this._next = new(next, false);
        }

        public ParsecStateStream(TPosition position, IDisposable? resource)
        {
            this.Current = default!;
            this.HasValue = false;
            this._position = position;
            this.InnerResource = resource;
            this._next = new(() => this, false);
        }

        public void Dispose()
            => this.InnerResource?.Dispose();

        public bool Equals(ParsecStateStream<TToken, TPosition>? other)
            => ReferenceEquals(this, other);

        public sealed override bool Equals(object? obj)
            => ReferenceEquals(this, obj);

        public sealed override int GetHashCode()
            => base.GetHashCode();

        public sealed override string ToString()
            => this.HasValue
                ? this.Current?.ToString() ?? string.Empty
                : "<EndOfStream>";
    }
}
