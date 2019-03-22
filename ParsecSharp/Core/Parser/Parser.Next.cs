using System;

namespace ParsecSharp.Internal
{
    internal sealed class Next<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        private readonly Func<TIntermediate, Parser<TToken, T>> _success;

        private readonly Func<Fail<TToken, TIntermediate>, Parser<TToken, T>> _fail;

        internal Next(Parser<TToken, TIntermediate> parser, Func<TIntermediate, Parser<TToken, T>> success, Func<Fail<TToken, TIntermediate>, Parser<TToken, T>> fail)
        {
            this._parser = parser;
            this._success = success;
            this._fail = fail;
        }

        protected internal sealed override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _success = this._success;
            var _fail = this._fail;
            return this._parser.Run(state, result => result.CaseOf(
                fail => _fail(fail).Run(state, cont),
                success => success.Next(_success, cont)));
        }
    }
}
