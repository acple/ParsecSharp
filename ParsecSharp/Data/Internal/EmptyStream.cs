namespace ParsecSharp.Internal
{
    public sealed class EmptyStream<TToken> : IParsecStateStream<TToken>
    {
        public static IParsecStateStream<TToken> Instance { get; } = new EmptyStream<TToken>();

        public TToken Current => default;

        public bool HasValue => false;

        public IPosition Position => LinearPosition.Initial;

        public IParsecStateStream<TToken> Next => this;

        private EmptyStream()
        { }

        public void Dispose()
        { }

        public override string ToString()
            => string.Empty;
    }
}
