using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParsecSharp.Internal
{
    public sealed class Buffer<TToken> : IReadOnlyList<TToken>
    {
        public static Buffer<TToken> Empty { get; } = new(Array.Empty<TToken>(), () => Empty!);

        private readonly TToken[] _buffer;

        private readonly int _index;

        private readonly Lazy<Buffer<TToken>> _next;

        public TToken this[int index] => this._buffer[this._index + index];

        public int Count { get; }

        public Buffer<TToken> Next => this._next.Value;

        public Buffer(TToken[] buffer, Func<Buffer<TToken>> next) : this(buffer, 0, buffer.Length, next)
        { }

        public Buffer(TToken[] buffer, int index, int length, Func<Buffer<TToken>> next)
        {
            this._buffer = buffer;
            this._index = index;
            this.Count = length;
            this._next = new(next, false);
        }

        IEnumerator<TToken> IEnumerable<TToken>.GetEnumerator()
            => new ArraySegment<TToken>(this._buffer, this._index, this.Count).AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.AsEnumerable().GetEnumerator();
    }
}
