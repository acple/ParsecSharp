using System;
using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateEnumerator<TToken, TState>(TState source) : IEnumerator<TState>
        where TState : IParsecState<TToken, TState>
    {
        public TState Current { get; private set; } = default!;

        object? IEnumerator.Current => this.Current;

        public bool MoveNext()
            => (this.Current = this.Current?.HasValue == true ? this.Current.Next : source).HasValue; // this definition violates IEnumerator implementation guidelines deliberately

        void IEnumerator.Reset()
            => throw new NotSupportedException();

        public void Dispose()
            => this.Current = default!;
    }
}
