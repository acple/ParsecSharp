using System;
using System.Runtime.CompilerServices;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Pure<T>(T value)
            => Pure<char, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Pure<T>(Func<T> value)
            => Pure<char, T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Fail<T>()
            => Fail<char, T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Fail<T>(string message)
            => Fail<char, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Fail<T>(Func<IParsecState<char>, string> message)
            => Fail<char, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Abort<T>(Func<IParsecState<char>, string> message)
            => Abort<char, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Abort<T>(Exception exception)
            => Abort<char, T>(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, IPosition> GetPosition()
            => GetPosition<char>();
    }
}
