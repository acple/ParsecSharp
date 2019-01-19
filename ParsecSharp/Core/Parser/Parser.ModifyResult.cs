using System;

namespace Parsec.Internal
{
    internal class ModifyResult<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        private readonly Func<IParsecStateStream<TToken>, Fail<TToken, TIntermediate>, Result<TToken, T>> _fail;

        private readonly Func<IParsecStateStream<TToken>, Success<TToken, TIntermediate>, Result<TToken, T>> _success;

        internal ModifyResult(Parser<TToken, TIntermediate> parser, Func<IParsecStateStream<TToken>, Fail<TToken, TIntermediate>, Result<TToken, T>> fail, Func<IParsecStateStream<TToken>, Success<TToken, TIntermediate>, Result<TToken, T>> success)
        {
            this._parser = parser;
            this._fail = fail;
            this._success = success;
        }

        internal override Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _fail = this._fail;
            var _success = this._success;
            return this._parser.Run(state, result => result.CaseOf(
                fail => cont(_fail(state, fail)),
                success => cont(_success(state, success))));
        }
    }
}
