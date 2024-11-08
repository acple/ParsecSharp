using System;

namespace ParsecSharp.Internal
{
    public abstract class ModifyResult<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser) : Parser<TToken, T>
    {
        protected abstract IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, TIntermediate> failure)
            where TState : IParsecState<TToken, TState>;

        protected abstract IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, TIntermediate> success)
            where TState : IParsecState<TToken, TState>;

        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => result.CaseOf(
                failure => cont(this.Fail(state, failure)),
                success => cont(this.Succeed(state, success))));
    }
}
