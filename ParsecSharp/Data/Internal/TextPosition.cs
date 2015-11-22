namespace Parsec.Internal
{
    internal struct TextPosition : IPosition
    {
        public static TextPosition Initial => new TextPosition(1, 1);

        public int Line { get; }

        public int Column { get; }

        private TextPosition(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public TextPosition Next(char token)
            => (token == '\n')
                ? new TextPosition(this.Line + 1, 1)
                : new TextPosition(this.Line, this.Column + 1);

        public override string ToString()
            => $"Line: { this.Line }, Column: { this.Column }";
    }
}
