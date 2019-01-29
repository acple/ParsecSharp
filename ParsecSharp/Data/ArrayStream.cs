using System.Collections.Generic;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ArrayStream<TToken> : IParsecStateStream<TToken>
    {
        private readonly IReadOnlyList<TToken> _source;

        private readonly LinearPosition _position;

        public TToken Current => this._source[this._position.Column];

        public bool HasValue => this._position.Column < this._source.Count;

        public IPosition Position => this._position;

        public IParsecStateStream<TToken> Next => new ArrayStream<TToken>(this._source, this._position.Next());

        public ArrayStream(IReadOnlyList<TToken> source) : this(source, LinearPosition.Initial)
        { }

        private ArrayStream(IReadOnlyList<TToken> source, LinearPosition position)
        {
            this._source = source;
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
