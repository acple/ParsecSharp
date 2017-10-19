using System;

namespace Parsec.Internal
{
    internal class Bind<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        private readonly Func<TIntermediate, Parser<TToken, T>> _next;

        internal Bind(Parser<TToken, TIntermediate> parser, Func<TIntermediate, Parser<TToken, T>> next)
        {
            this._parser = parser;
            this._next = next;
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _next = this._next;
            return this._parser.Run(state, result => result.Next(_next, cont));
        }
    }
}
