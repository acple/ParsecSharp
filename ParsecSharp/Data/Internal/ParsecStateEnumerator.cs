using System;
using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateEnumerator<TToken, TState> : IEnumerator<TState>
        where TState : IParsecState<TToken, TState>
    {
        private readonly TState _source;

        public TState Current { get; private set; }

        object? IEnumerator.Current => this.Current;

        public ParsecStateEnumerator(TState source)
        {
            this._source = source;
            this.Current = default!;
        }

        public bool MoveNext()
            => (this.Current = (this.Current?.HasValue == true) ? this.Current.Next : this._source).HasValue; // this definition violates IEnumerator implementation guidelines deliberately

        void IEnumerator.Reset()
            => throw new NotSupportedException();

        public void Dispose()
            => this.Current = default!;
    }
}
