using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Resume<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _parser;

        private readonly Func<Failure<TToken, T>, Parser<TToken, T>> _resume;

        public Resume(Parser<TToken, T> parser, Func<Failure<TToken, T>, Parser<TToken, T>> resume)
        {
            this._parser = parser;
            this._resume = resume;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _resume = this._resume;
            return this._parser.Run(state, result => result.CaseOf(failure => _resume(failure).Run(state, cont), cont));
        }
    }
}
