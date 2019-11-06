using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ByteStream : IParsecState<byte, ByteStream>
    {
        private const int MaxBufferSize = 2048;

        private readonly Buffer<byte> _buffer;

        private readonly LinearPosition _position;

        private int Index => this._position.Column % MaxBufferSize;

        public byte Current => this._buffer[this.Index];

        public bool HasValue => this.Index < this._buffer.Count;

        public IPosition Position => this._position;

        public IDisposable InnerResource { get; }

        public ByteStream Next => new ByteStream(this.InnerResource, (this.Index == MaxBufferSize - 1) ? this._buffer.Next : this._buffer, this._position.Next());

        public ByteStream(Stream source) : this(source, CreateBuffer(source), LinearPosition.Initial)
        { }

        private ByteStream(IDisposable source, Buffer<byte> buffer, LinearPosition position)
        {
            this.InnerResource = source;
            this._buffer = buffer;
            this._position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Buffer<byte> CreateBuffer(Stream stream)
        {
            try
            {
                var buffer = Enumerable.Repeat(stream, MaxBufferSize)
                    .Select(stream => stream.ReadByte())
                    .TakeWhile(x => x != -1)
                    .Select(x => (byte)x)
                    .ToArray();
                return new Buffer<byte>(buffer, () => CreateBuffer(stream));
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public IParsecState<byte> GetState()
            => this;

        public void Dispose()
            => this.InnerResource.Dispose();

        public bool Equals(ByteStream other)
            => this._buffer == other._buffer && this._position == other._position;

        public sealed override bool Equals(object? obj)
            => obj is ByteStream state && this._buffer == state._buffer && this._position == state._position;

        public sealed override int GetHashCode()
            => this._buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue) ? this.Current.ToString() : "<EndOfStream>";
    }
}
