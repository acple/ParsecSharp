using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Pure<TToken, T>(T value)
            => new Pure<TToken, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Pure<TToken, T>(Func<IParsecState<TToken>, T> value)
            => new PureDelayed<TToken, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>()
            => new ParseError<TToken, T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>(string message)
            => new FailWithMessage<TToken, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>(Func<IParsecState<TToken>, string> message)
            => new FailWithMessageDelayed<TToken, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Abort<TToken, T>(Func<IParsecState<TToken>, string> message)
            => new Terminate<TToken, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Abort<TToken, T>(Exception exception)
            => new Abort<TToken, T>(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IPosition> GetPosition<TToken>()
            => new GetPosition<TToken>();
    }
}
