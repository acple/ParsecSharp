using System;
using System.Collections;
using System.Collections.Generic;

namespace ParsecSharp.Internal
{
    public sealed class ParsecStateEnumerator<TToken, TState>(TState source) : IEnumerator<TState>
        where TState : IParsecState<TToken, TState>
    {
        private TState? current;

        public TState Current => this.current!;

        object? IEnumerator.Current => this.current;

        public bool MoveNext()
            => (this.current = this.current is { HasValue: true } current ? current.Next : source).HasValue; // this definition violates IEnumerator implementation guidelines deliberately

        void IEnumerator.Reset()
            => throw new NotSupportedException();

        public void Dispose()
            => this.current = default;
    }
}
