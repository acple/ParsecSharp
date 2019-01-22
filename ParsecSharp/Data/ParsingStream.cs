using System;
using System.Collections;
using System.Collections.Generic;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public sealed class ParsingStream<TToken, T> : IParsecStateStream<T>
    {
        private readonly IDisposable disposable;

        private readonly LinearPosition _position;

        private readonly Lazy<IParsecStateStream<T>> _next;

        public T Current { get; }

        public bool HasValue { get; }

        public IPosition Position => this._position;

        public IParsecStateStream<T> Next => this._next.Value;

        public ParsingStream(Parser<TToken, T> parser, IParsecStateStream<TToken> source) : this(parser, LinearPosition.Initial, source)
        { }

        private ParsingStream(Parser<TToken, T> parser, LinearPosition position, IParsecStateStream<TToken> source)
        {
            var (result, rest) = parser.ParsePartially(source);
            this.disposable = rest;
            this._position = position;
            this.HasValue = result.CaseOf(_ => false, _ => true);
            this.Current = (this.HasValue) ? result.Value : default;
            this._next = new Lazy<IParsecStateStream<T>>(() => new ParsingStream<TToken, T>(parser, position.Next(), rest), false);
        }

        public void Dispose()
            => this.disposable.Dispose();

        public sealed override string ToString()
            => (this.HasValue)
                ? this.Current?.ToString() ?? string.Empty
                : string.Empty;
    }
}
