using System;
using System.Collections.Generic;

namespace Parsec.Internal
{
    public sealed class EnumerableStream<T> : IParsecStateStream<T>
    {
        private readonly IDisposable _disposable;

        private readonly Lazy<IParsecStateStream<T>> _next;

        private readonly LinearPosition _position;

        public T Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<T> Next => this._next.Value;

        public EnumerableStream(IEnumerable<T> source) : this(source.GetEnumerator())
        { }

        public EnumerableStream(IEnumerator<T> enumerator) : this(enumerator, LinearPosition.Initial)
        { }

        private EnumerableStream(IEnumerator<T> enumerator, LinearPosition position)
        {
            this._disposable = enumerator;
            this._position = position;
            try
            {
                this.HasValue = enumerator.MoveNext();
                this.Current = (this.HasValue) ? enumerator.Current : default(T);
                this._next = new Lazy<IParsecStateStream<T>>(
                    () => new EnumerableStream<T>(enumerator, this._position.Next()), false);
            }
            catch
            {
                this.HasValue = false;
                this.Current = default(T);
                this._next = new Lazy<IParsecStateStream<T>>(() => EmptyStream<T>.Instance, false);
                this.Dispose();
                throw;
            }
        }

        public void Dispose()
            => this._disposable.Dispose();

        public override string ToString()
            => this.Current.ToString();

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => new ParsecStateStreamEnumerator<T>(this);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => new ParsecStateStreamEnumerator<T>(this);
    }
}
