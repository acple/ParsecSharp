using System;

namespace ParsecSharp.Internal
{
    internal sealed class Resume<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _parser;

        private readonly Func<Fail<TToken, T>, Parser<TToken, T>> _resume;

        internal Resume(Parser<TToken, T> parser, Func<Fail<TToken, T>, Parser<TToken, T>> resume)
        {
            this._parser = parser;
            this._resume = resume;
        }

        protected internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _resume = this._resume;
            return this._parser.Run(state, result => result.CaseOf(fail => _resume(fail).Run(state, cont), cont));
        }
    }
}
