using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    public static class Builder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Create<TToken, T>(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
            => new Single<TToken, T>(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> ModifyResult<TToken, T, TResult>(this Parser<TToken, T> parser, Func<IParsecStateStream<TToken>, Fail<TToken, T>, Result<TToken, TResult>> fail, Func<IParsecStateStream<TToken>, Success<TToken, T>, Result<TToken, TResult>> success)
            => new ModifyResult<TToken, T, TResult>(parser, fail, success);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IParsecStateStream<TToken>> GetState<TToken>()
            => Create<TToken, IParsecStateStream<TToken>>(state => Result.Success(state, state));
    }
}
