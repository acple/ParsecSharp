namespace ParsecSharp.Internal
{
    public readonly struct EmptyPosition : IPosition
    {
        public static EmptyPosition Initial => default;

        public int Line => 0;

        public int Column => -1;

        public int CompareTo(IPosition other)
            => (other is EmptyPosition) ? 0 : -1;

        public bool Equals(IPosition other)
            => other is EmptyPosition;

        public override bool Equals(object? obj)
            => obj is EmptyPosition;

        public override int GetHashCode()
            => 0;

        public override string ToString()
            => "Position: none";
    }
}
