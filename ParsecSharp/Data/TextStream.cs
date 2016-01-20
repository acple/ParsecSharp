using System;
using System.Collections.Generic;
using System.IO;
using Parsec.Internal;

namespace Parsec
{
    public sealed class TextStream : IParsecStateStream<char>
    {
        private readonly IDisposable _disposable;

        private readonly TextPosition _position;

        private readonly Lazy<IParsecStateStream<char>> _next;

        public char Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<char> Next => this._next.Value;

        public TextStream(Stream stream) : this(new StreamReader(stream))
        { }

        public TextStream(TextReader reader) : this(reader, TextPosition.Initial)
        { }

        private TextStream(TextReader reader, TextPosition position)
        {
            this._disposable = reader;
            this._position = position;
            try
            {
                var token = reader.Read();
                this.HasValue = token != -1;
                this.Current = (this.HasValue) ? (char)token : default(char);
                this._next = new Lazy<IParsecStateStream<char>>(() => new TextStream(reader, position.Next(this.Current)), false);
            }
            catch
            {
                this.HasValue = false;
                this.Current = default(char);
                this._next = new Lazy<IParsecStateStream<char>>(() => EmptyStream<char>.Instance, false);
                this.Dispose();
                throw;
            }
        }

        public void Dispose()
            => this._disposable.Dispose();

        public override string ToString()
            => this.Current.ToString();

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
            => new ParsecStateStreamEnumerator<char>(this);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => new ParsecStateStreamEnumerator<char>(this);
    }
}
