namespace Parsec.Internal
{
    internal struct LinearPosition : IPosition
    {
        public static LinearPosition Initial => new LinearPosition(0);

        public int Line => 1;

        public int Column { get; }

        private LinearPosition(int index)
        {
            this.Column = index;
        }

        public LinearPosition Next()
            => new LinearPosition(this.Column + 1);

        public override string ToString()
            => $"Position: { this.Column }";
    }
}
