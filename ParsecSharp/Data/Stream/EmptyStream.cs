using System;

namespace ParsecSharp.Internal
{
    public sealed class EmptyStream<TToken> : IParsecState<TToken, EmptyStream<TToken>>
    {
        public static EmptyStream<TToken> Instance { get; } = new();

        public TToken Current => default!;

        public bool HasValue => false;

        public IPosition Position => EmptyPosition<TToken>.Initial;

        public IDisposable? InnerResource => default;

        public EmptyStream<TToken> Next => this;

        private EmptyStream()
        { }

        public IParsecState<TToken> GetState()
            => this;

        public void Dispose()
        { }

        public bool Equals(EmptyStream<TToken>? other)
            => other is not null;

        public sealed override bool Equals(object? obj)
            => ReferenceEquals(this, obj);

        public sealed override int GetHashCode()
            => 0;

        public sealed override string ToString()
            => "<EndOfStream>";
    }
}
