namespace ParsecSharp.Internal
{
    public readonly struct LinearPosition : IPosition
    {
        public static LinearPosition Initial => default;

        public int Line => 0;

        public int Column { get; }

        public LinearPosition(int index)
        {
            this.Column = index;
        }

        public LinearPosition Next()
            => new LinearPosition(this.Column + 1);

        public int CompareTo(IPosition other)
            => (this.Line != other.Line) ? this.Line.CompareTo(other.Line) : this.Column.CompareTo(other.Column);

        public bool Equals(IPosition other)
            => other is LinearPosition position && this == position;

        public override bool Equals(object obj)
            => obj is LinearPosition position && this == position;

        public override int GetHashCode()
            => this.Column.GetHashCode();

        public override string ToString()
            => $"Index: {this.Column.ToString()}";

        public static bool operator ==(LinearPosition left, LinearPosition right)
            => left.Column == right.Column;

        public static bool operator !=(LinearPosition left, LinearPosition right)
            => !(left == right);
    }
}
