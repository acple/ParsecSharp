using System;
using System.Runtime.CompilerServices;

namespace Parsec.Internal
{
    public static class Builder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Create<TToken, T>(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
            => new Single<TToken, T>(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> ModifyResult<TToken, T, TResult>(this Parser<TToken, T> parser, Func<IParsecStateStream<TToken>, Fail<TToken, T>, Result<TToken, TResult>> fail, Func<IParsecStateStream<TToken>, Success<TToken, T>, Result<TToken, TResult>> success)
            => new ModifyResult<TToken, T, TResult>(parser, fail, success);
    }
}
