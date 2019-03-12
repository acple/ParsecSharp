using ParsecSharp.Internal;

namespace ParsecSharp
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

        public bool Equals(IParsecState<char> other)
            => other is StringStream state && this._source == state._source && this._index == state._index;

        public sealed override bool Equals(object obj)
            => obj is StringStream state && this._source == state._source && this._index == state._index;

        public sealed override int GetHashCode()
            => this._source.GetHashCode() ^ this._index.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue) ? this.Current.ToReadableStringWithCharCode() : "<EndOfStream>";
    }
}
