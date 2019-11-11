using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Bind<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        private readonly Func<TIntermediate, Parser<TToken, T>> _next;

        public Bind(Parser<TToken, TIntermediate> parser, Func<TIntermediate, Parser<TToken, T>> next)
        {
            this._parser = parser;
            this._next = next;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _next = this._next;
            return this._parser.Run(state, result => result.Next(_next, cont));
        }
    }
}
