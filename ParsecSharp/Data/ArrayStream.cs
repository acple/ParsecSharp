using System.Collections.Generic;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ArrayStream<TToken> : IParsecStateStream<TToken>
    {
        private readonly IReadOnlyList<TToken> _source;

        private readonly int _index;

        private readonly LinearPosition _position;

        public TToken Current => this._source[this._index];

        public bool HasValue => this._index < this._source.Count;

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => new ArrayStream<TToken>(this._source, this._index + 1, this._position.Next());

        public ArrayStream(IReadOnlyList<TToken> source) : this(source, 0, LinearPosition.Initial)
        { }

        private ArrayStream(IReadOnlyList<TToken> source, int index, LinearPosition position)
        {
            this._source = source;
            this._index = index;
            this._position = position;
        }

        public void Dispose()
        { }

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : string.Empty;
    }
}
