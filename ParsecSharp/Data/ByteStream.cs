using System;
using System.IO;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ByteStream : IParsecStateStream<byte>
    {
        private readonly IDisposable disposable;

        private readonly LinearPosition _position;

        private readonly Lazy<IParsecStateStream<byte>> _next;

        public byte Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<byte> Next => this._next.Value;

        public ByteStream(Stream source) : this(source, LinearPosition.Initial)
        { }

        private ByteStream(Stream source, LinearPosition position)
        {
            this.disposable = source;
            this._position = position;
            try
            {
                var token = source.ReadByte();
                this.HasValue = token != -1;
                this.Current = (this.HasValue) ? (byte)token : default;
            }
            catch
            {
                this.Dispose();
                throw;
            }
            finally
            {
                this._next = new Lazy<IParsecStateStream<byte>>(() => new ByteStream(source, position.Next()), false);
            }
        }

        public void Dispose()
            => this.disposable.Dispose();

        public bool Equals(IParsecState<byte> other)
            => ReferenceEquals(this, other);

        public sealed override string ToString()
            => (this.HasValue) ? this.Current.ToString() : "<EndOfStream>";
    }
}
