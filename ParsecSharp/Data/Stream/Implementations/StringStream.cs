using System;
using ParsecSharp.Internal;

namespace ParsecSharp.Data
{
    public sealed class StringStream<TPosition> : IParsecState<char, StringStream<TPosition>>
        where TPosition : IPosition<char, TPosition>
    {
        private readonly string _source;

        private readonly int _index;

        private readonly TPosition _position;

        public char Current => this._source[this._index];

        public bool HasValue => this._index < this._source.Length;

        public IPosition Position => this._position;

        public IDisposable? InnerResource => default;

        public StringStream<TPosition> Next => new(this._source, this._index + 1, this._position.Next(this.Current));

        public StringStream(string source, TPosition position) : this(source, index: 0, position)
        { }

        private StringStream(string source, int index, TPosition position)
        {
            this._source = source;
            this._index = index;
            this._position = position;
        }

        public void Dispose()
        { }

        public bool Equals(StringStream<TPosition>? other)
            => other is not null && this._source == other._source && this._index == other._index;

        public sealed override bool Equals(object? obj)
            => obj is StringStream<TPosition> state && this._source == state._source && this._index == state._index;

        public sealed override int GetHashCode()
            => this._source.GetHashCode() ^ this._index;

        public sealed override string ToString()
            => this.HasValue ? CharConvert.ToReadableStringWithCharCode(this.Current) : "<EndOfStream>";
    }
}
