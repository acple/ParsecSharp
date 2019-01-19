namespace Parsec.Internal
{
    public readonly struct TextPosition : IPosition
    {
        public static TextPosition Initial => new TextPosition(1, 1);

        public int Line { get; }

        public int Column { get; }

        public TextPosition(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public TextPosition Next(char token)
            => (token == '\n')
                ? new TextPosition(this.Line + 1, 1)
                : new TextPosition(this.Line, this.Column + 1);

        public override string ToString()
            => $"Line: {this.Line.ToString()}, Column: {this.Column.ToString()}";
    }
}
