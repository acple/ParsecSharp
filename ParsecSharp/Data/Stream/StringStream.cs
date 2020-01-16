using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static class StringStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringStream<TextPosition> Create(string source)
            => Create(source, TextPosition.Initial);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringStream<TPosition> Create<TPosition>(string source, TPosition position)
            where TPosition : IPosition<char, TPosition>, IEquatable<TPosition>
            => new StringStream<TPosition>(source, position);
    }
}

namespace ParsecSharp.Internal
{
    public sealed class StringStream<TPosition> : IParsecState<char, StringStream<TPosition>>
        where TPosition : IPosition<char, TPosition>, IEquatable<TPosition>
    {
        private readonly string _source;

        private readonly int _index;

        private readonly TPosition _position;

        public char Current => this._source[this._index];

        public bool HasValue => this._index < this._source.Length;

        public IPosition Position => this._position;

        public IDisposable? InnerResource => default;

        public StringStream<TPosition> Next => new StringStream<TPosition>(this._source, this._index + 1, this._position.Next(this.Current));

        public StringStream(string source, TPosition position) : this(source, 0, position)
        { }

        private StringStream(string source, int index, TPosition position)
        {
            this._source = source;
            this._index = index;
            this._position = position;
        }

        public IParsecState<char> GetState()
            => this;

        public void Dispose()
        { }

        public bool Equals(StringStream<TPosition> other)
            => this._source == other._source && this._index == other._index;

        public sealed override bool Equals(object? obj)
            => obj is StringStream<TPosition> state && this._source == state._source && this._index == state._index;

        public sealed override int GetHashCode()
            => this._source.GetHashCode() ^ this._index.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue) ? CharConvert.ToReadableStringWithCharCode(this.Current) : "<EndOfStream>";
    }
}
