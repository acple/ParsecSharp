using System;
using System.IO;
using System.Linq;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ByteStream : IParsecStateStream<byte>
    {
        private const int MaxBufferSize = 1024;

        private sealed class Buffer : IDisposable
        {
            private readonly IDisposable disposable;

            private readonly Lazy<Buffer> _next;

            public byte[] Current { get; }

            public Buffer Next => this._next.Value;

            public Buffer(Stream stream)
            {
                this.disposable = stream;
                try
                {
                    this.Current = Enumerable.Repeat(stream, MaxBufferSize)
                        .Select(stream => stream.ReadByte())
                        .TakeWhile(x => x != -1)
                        .Select(x => (byte)x)
                        .ToArray();
                }
                catch
                {
                    this.Dispose();
                    throw;
                }
                finally
                {
                    this._next = new Lazy<Buffer>(() => new Buffer(stream));
                }
            }

            public void Dispose()
                => this.disposable.Dispose();
        }

        private readonly Buffer buffer;

        private readonly LinearPosition _position;

        private int Index => this._position.Column % MaxBufferSize;

        public byte Current => this.buffer.Current[this.Index];

        public bool HasValue => this.Index < this.buffer.Current.Length;

        public IPosition Position => this._position;

        public IParsecStateStream<byte> Next => new ByteStream((this.Index == MaxBufferSize - 1) ? this.buffer.Next : this.buffer, this._position.Next());

        public ByteStream(Stream source) : this(new Buffer(source), LinearPosition.Initial)
        { }

        private ByteStream(Buffer buffer, LinearPosition position)
        {
            this.buffer = buffer;
            this._position = position;
        }

        public void Dispose()
            => this.buffer.Dispose();

        public bool Equals(IParsecState<byte> other)
            => other is ByteStream state && this.buffer == state.buffer && this._position == state._position;

        public sealed override bool Equals(object obj)
            => obj is ByteStream state && this.buffer == state.buffer && this._position == state._position;

        public sealed override int GetHashCode()
            => this.buffer.GetHashCode() ^ this._position.GetHashCode();

        public sealed override string ToString()
            => (this.HasValue) ? this.Current.ToString() : "<EndOfStream>";
    }
}
