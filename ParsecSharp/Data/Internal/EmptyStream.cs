namespace ParsecSharp.Internal
{
    public sealed class EmptyStream<TToken> : IParsecStateStream<TToken>
    {
        public static IParsecStateStream<TToken> Instance { get; } = new EmptyStream<TToken>();

        public TToken Current => default!;

        public bool HasValue => false;

        public IPosition Position => LinearPosition.Initial;

        public IParsecStateStream<TToken> Next => this;

        private EmptyStream()
        { }

        public void Dispose()
        { }

        public bool Equals(IParsecState<TToken> other)
            => other is EmptyStream<TToken>;

        public sealed override bool Equals(object obj)
            => obj is EmptyStream<TToken>;

        public sealed override int GetHashCode()
            => 0;

        public sealed override string ToString()
            => "<EndOfStream>";
    }
}
