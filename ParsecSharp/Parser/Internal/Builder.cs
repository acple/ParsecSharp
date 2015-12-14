using System;

namespace Parsec.Internal
{
    public static class Builder
    {
        public static Parser<TToken, T> Create<TToken, T>(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
            => new Parser<TToken, T>(function);

        public static Result<TToken, T> Run<TToken, T>(this Parser<TToken, T> parser, IParsecStateStream<TToken> state)
            => parser.Run(state);

        public static Parser<TToken, TResult> ModifyResult<TToken, T, TResult>(this Parser<TToken, T> parser, Func<IParsecStateStream<TToken>, Fail<TToken, T>, Result<TToken, TResult>> failHandler, Func<IParsecStateStream<TToken>, Success<TToken, T>, Result<TToken, TResult>> successHandler)
            => Create<TToken, TResult>(state => parser.Run(state).CaseOf(
                fail => failHandler(state, fail),
                success => successHandler(state, success)));
    }
}
