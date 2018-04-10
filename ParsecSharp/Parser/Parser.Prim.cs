using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, TToken> Any<TToken>()
            => Satisfy<TToken>(_ => true);

        public static Parser<TToken, Unit> EndOfInput<TToken>()
            => Not(Any<TToken>());

        public static Parser<TToken, TToken> OneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(x => candidates.Contains(x));

        public static Parser<TToken, TToken> OneOf<TToken>(params TToken[] candidates)
            => OneOf(candidates.AsEnumerable());

        public static Parser<TToken, TToken> NoneOf<TToken>(IEnumerable<TToken> candidates)
            => Satisfy<TToken>(x => !candidates.Contains(x));

        public static Parser<TToken, TToken> NoneOf<TToken>(params TToken[] candidates)
            => NoneOf(candidates.AsEnumerable());

        public static Parser<TToken, TToken> Satisfy<TToken>(Func<TToken, bool> predicate)
            => Builder.Create<TToken, TToken>(state => (state.HasValue && predicate(state.Current))
                ? Result.Success(state.Current, state.Next)
                : Result.Fail<TToken, TToken>(state));
    }
}
