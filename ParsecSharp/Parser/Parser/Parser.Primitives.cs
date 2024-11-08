using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> Any<TToken>()
            => Satisfy<TToken>(_ => true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> Token<TToken>(TToken token)
            => Satisfy<TToken>(x => EqualityComparer<TToken>.Default.Equals(x, token));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> EndOfInput<TToken>()
            => Not(Any<TToken>()).WithMessage(failure => $"Expected '<EndOfStream>' but was '{failure.State.ToString()}'");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> Null<TToken>()
            => Pure<TToken, Unit>(Unit.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> Condition<TToken>(bool success)
            => Condition<TToken>(success, "Given condition was false");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> Condition<TToken>(bool success, string message)
            => success ? Null<TToken>() : Fail<TToken, Unit>(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> OneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(candidates.Contains);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> OneOf<TToken>(params TToken[] candidates)
            => OneOf(candidates.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> NoneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(x => !candidates.Contains(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> NoneOf<TToken>(params TToken[] candidates)
            => NoneOf(candidates.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> Take<TToken>(int count)
            => new Take<TToken>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> TakeWhile<TToken>(Func<TToken, bool> predicate)
            => Many(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> TakeWhile1<TToken>(Func<TToken, bool> predicate)
            => Many1(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> Skip<TToken>(int count)
            => new Skip<TToken>(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> SkipWhile<TToken>(Func<TToken, bool> predicate)
            => SkipMany(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> SkipWhile1<TToken>(Func<TToken, bool> predicate)
            => SkipMany1(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, TToken> Satisfy<TToken>(Func<TToken, bool> predicate)
            => new Satisfy<TToken>(predicate);
    }
}
