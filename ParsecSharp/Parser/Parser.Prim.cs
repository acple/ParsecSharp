using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> Any<TToken>()
            => Satisfy<TToken>(_ => true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> EndOfInput<TToken>()
            => Not(Any<TToken>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> OneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(candidates.Contains);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> OneOf<TToken>(params TToken[] candidates)
            => OneOf(candidates.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> NoneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(x => !candidates.Contains(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> NoneOf<TToken>(params TToken[] candidates)
            => NoneOf(candidates.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> Take<TToken>(int count)
            => Any<TToken>().Repeat(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> TakeWhile<TToken>(Func<TToken, bool> predicate)
            => Many(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> TakeWhile1<TToken>(Func<TToken, bool> predicate)
            => Many1(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> Skip<TToken>(int count)
            => Builder.Create<TToken, Unit>(state => (state.AsEnumerable().Take(count).Count() == count)
                ? Result.Success(Unit.Instance, state.Advance(count))
                : Result.Fail<TToken, Unit>(state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> SkipWhile<TToken>(Func<TToken, bool> predicate)
            => SkipMany(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> SkipWhile1<TToken>(Func<TToken, bool> predicate)
            => SkipMany1(Satisfy(predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TToken> Satisfy<TToken>(Func<TToken, bool> predicate)
            => Builder.Create<TToken, TToken>(state => (state.HasValue && predicate(state.Current))
                ? Result.Success(state.Current, state.Next)
                : Result.Fail<TToken, TToken>(state));
    }
}
