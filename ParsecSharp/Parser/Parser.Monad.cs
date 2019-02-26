using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Pure<TToken, T>(T value)
            => Builder.Create<TToken, T>(state => Result.Success(value, state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Pure<TToken, T>(Func<T> value)
            => Builder.Create<TToken, T>(state => Result.Success(value(), state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>()
            => Builder.Create<TToken, T>(state => Result.Fail<TToken, T>(state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>(string message)
            => Builder.Create<TToken, T>(state => Result.Fail<TToken, T>(message, state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fail<TToken, T>(Func<IParsecState<TToken>, string> message)
            => Builder.Create<TToken, T>(state => Result.Fail<TToken, T>(message(state), state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Abort<TToken, T>(Func<IParsecState<TToken>, string> message)
            => new Terminate<TToken, T>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Abort<TToken, T>(Exception exception)
            => new Abort<TToken, T>(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IPosition> GetPosition<TToken>()
            => Builder.Create<TToken, IPosition>(state => Result.Success(state.Position, state));
    }
}
