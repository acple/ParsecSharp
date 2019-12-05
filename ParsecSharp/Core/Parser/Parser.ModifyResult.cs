using System;

namespace ParsecSharp.Internal
{
    public abstract class ModifyResult<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        protected ModifyResult(Parser<TToken, TIntermediate> parser)
        {
            this._parser = parser;
        }

        protected abstract Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIntermediate> failure)
            where TState : IParsecState<TToken, TState>;

        protected abstract Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIntermediate> success)
            where TState : IParsecState<TToken, TState>;

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Run(state, result => result.CaseOf(
                failure => cont(this.Fail(state, failure)),
                success => cont(this.Succeed(state, success))));
    }
}
