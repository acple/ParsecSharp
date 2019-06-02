using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class TextStream : IParsecStateStream<char>
    {
        private const int MaxBufferSize = 2048;

        private readonly IDisposable source;

        private readonly Buffer<char> _buffer;

        private readonly int _index;

        private readonly TextPosition _position;

        public char Current => this._buffer[this._index];

        public bool HasValue => this._index < this._buffer.Count;

        public IPosition Position => this._position;

        public IParsecStateStream<char> Next => (this._index == MaxBufferSize - 1)
            ? new TextStream(this.source, this._buffer.Next, 0, this._position.Next(this.Current))
            : new TextStream(this.source, this._buffer, this._index + 1, this._position.Next(this.Current));

        public TextStream(Stream source) : this(source, Encoding.UTF8)
        { }

        public TextStream(Stream source, Encoding encoding) : this(new StreamReader(source, encoding))
        { }

        public TextStream(TextReader reader) : this(reader, CreateBuffer(reader), 0, TextPosition.Initial)
        { }

        private TextStream(IDisposable source, Buffer<char> buffer, int index, TextPosition position)
        {
            this.source = source;
            this._buffer = buffer;
            this._index = index;
            this._position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Buffer<char> CreateBuffer(TextReader reader)
        {
            try
            {
                var buffer = Enumerable.Repeat(reader, MaxBufferSize)
                    .Select(reader => reader.Read())
                    .TakeWhile(x => x != -1)
                    .Select(x => (char)x)
                    .ToArray();
                return new Buffer<char>(buffer, () => CreateBuffer(reader));
            }
            catch
            {
                reader.Dispose();
                throw;
            }
        }

        public void Dispose()
            => this.source.Dispose();

        public bool Equals(IParsecState<char> other)
            => other is TextStream state && this._buffer == state._buffer && this._position == state._position;

        public sealed override bool Equals(object obj)
            => obj is TextStream state && this._buffer == state._buffer && this._position == state._position;

        public sealed override int GetHashCode()
            => this._buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue) ? this.Current.ToReadableStringWithCharCode() : "<EndOfStream>";
    }
}
