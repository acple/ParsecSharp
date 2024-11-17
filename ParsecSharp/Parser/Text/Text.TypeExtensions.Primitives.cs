using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static ParsecSharp.Parser;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Any()
            => Any<char>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> EndOfInput()
            => EndOfInput<char>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Null()
            => Null<char>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Condition(bool success)
            => Condition<char>(success);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Condition(bool success, string message)
            => Condition<char>(success, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, IReadOnlyList<char>> Take(int count)
            => Take<char>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, IReadOnlyCollection<char>> TakeWhile(Func<char, bool> predicate)
            => TakeWhile<char>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, IReadOnlyCollection<char>> TakeWhile1(Func<char, bool> predicate)
            => TakeWhile1<char>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> Skip(int count)
            => Skip<char>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> SkipWhile(Func<char, bool> predicate)
            => SkipWhile<char>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, Unit> SkipWhile1(Func<char, bool> predicate)
            => SkipWhile1<char>(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, char> Satisfy(Func<char, bool> predicate)
            => Satisfy<char>(predicate);
    }
}
