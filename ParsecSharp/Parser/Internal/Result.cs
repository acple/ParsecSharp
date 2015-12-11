using System;

namespace Parsec.Internal
{
    public static class Result
    {
        public static Result<TToken, T> Fail<TToken, T>(IParsecState<TToken> state)
            => new ParseError<TToken, T>(state);

        public static Result<TToken, T> Fail<TToken, T>(Exception exception, IParsecState<TToken> state)
            => new FailWithException<TToken, T>(exception, state);

        public static Result<TToken, T> Fail<TToken, T>(string message, IParsecState<TToken> state)
            => new FailWithMessage<TToken, T>(message, state);

        public static Result<TToken, T> Success<TToken, T>(T value, IParsecStateStream<TToken> state)
            => new Success<TToken, T>(value, state);
    }
}
