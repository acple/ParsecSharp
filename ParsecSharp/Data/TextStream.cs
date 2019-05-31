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

        private readonly IDisposable resource;

        private readonly Buffer<char> _buffer;

        private readonly TextPosition _position;

        private readonly int _index;

        public char Current => this._buffer[this._index];

        public bool HasValue => this._index < this._buffer.Count;

        public IPosition Position => this._position;

        public IParsecStateStream<char> Next => (this._index == MaxBufferSize - 1)
            ? new TextStream(this.resource, this._buffer.Next, this._position.Next(this.Current), 0)
            : new TextStream(this.resource, this._buffer, this._position.Next(this.Current), this._index + 1);

        public TextStream(Stream source) : this(source, Encoding.UTF8)
        { }

        public TextStream(Stream source, Encoding encoding) : this(new StreamReader(source, encoding))
        { }

        public TextStream(TextReader reader) : this(reader, CreateBuffer(reader), TextPosition.Initial, 0)
        { }

        private TextStream(IDisposable resource, Buffer<char> buffer, TextPosition position, int index)
        {
            this.resource = resource;
            this._buffer = buffer;
            this._position = position;
            this._index = index;
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
            => this.resource.Dispose();

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
