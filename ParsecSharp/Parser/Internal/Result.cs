using System;
using System.Runtime.CompilerServices;

namespace Parsec.Internal
{
    public static class Result
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Fail<TToken, T>(IParsecStateStream<TToken> state)
            => new ParseError<TToken, T>(state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Fail<TToken, T>(Exception exception, IParsecStateStream<TToken> state)
            => new FailWithException<TToken, T>(exception, state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Fail<TToken, T>(string message, IParsecStateStream<TToken> state)
            => new FailWithMessage<TToken, T>(message, state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Success<TToken, T>(T value, IParsecStateStream<TToken> state)
            => new Success<TToken, T>(value, state);
    }
}
