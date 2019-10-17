using System;
using System.Runtime.CompilerServices;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Pure<T>(T value)
            => Pure<byte, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Pure<T>(Func<IParsecState<byte>, T> value)
            => Pure<byte, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Fail<T>()
            => Fail<byte, T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Fail<T>(string message)
            => Fail<byte, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Fail<T>(Func<IParsecState<byte>, string> message)
            => Fail<byte, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Abort<T>(Func<IParsecState<byte>, string> message)
            => Abort<byte, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Abort<T>(Exception exception)
            => Abort<byte, T>(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, IPosition> GetPosition()
            => GetPosition<byte>();
    }
}
