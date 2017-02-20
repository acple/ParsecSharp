using Parsec.Internal;

namespace Parsec
{
    public sealed class StringStream : IParsecStateStream<char>
    {
        private readonly string _source;

        private readonly int _index;

        private readonly TextPosition _position;

        public char Current => this._source[this._index];

        public bool HasValue => this._index < this._source.Length;

        public IPosition Position => this._position;

        public IParsecStateStream<char> Next => new StringStream(this._source, this._index + 1, this._position.Next(this.Current));

        public StringStream(string source) : this(source, 0, TextPosition.Initial)
        { }

        private StringStream(string source, int index, TextPosition position)
        {
            this._source = source;
            this._index = index;
            this._position = position;
        }

        public void Dispose()
        { }

        public override string ToString()
            => (this.HasValue) ? this.Current.ToString() : string.Empty;
    }
}
