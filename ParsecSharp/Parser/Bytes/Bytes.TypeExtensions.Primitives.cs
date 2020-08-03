using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, byte> Any()
            => Any<byte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> EndOfInput()
            => EndOfInput<byte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> Null()
            => Null<byte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> Condition(bool success)
            => Condition<byte>(success);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> Condition(bool success, string message)
            => Condition<byte>(success, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, IEnumerable<byte>> Take(int count)
            => Take<byte>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, IEnumerable<byte>> TakeWhile(Func<byte, bool> predicate)
            => TakeWhile<byte>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, IEnumerable<byte>> TakeWhile1(Func<byte, bool> predicate)
            => TakeWhile1<byte>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> Skip(int count)
            => Skip<byte>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> SkipWhile(Func<byte, bool> predicate)
            => SkipWhile<byte>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, Unit> SkipWhile1(Func<byte, bool> predicate)
            => SkipWhile1<byte>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, byte> Satisfy(Func<byte, bool> predicate)
            => Satisfy<byte>(predicate);
    }
}
