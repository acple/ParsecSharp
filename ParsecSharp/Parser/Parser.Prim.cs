using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<T, T> Any<T>()
            => Satisfy<T>(_ => true);

        public static Parser<T, Unit> EndOfInput<T>()
            => Not(Any<T>());

        public static Parser<T, T> OneOf<T>(IEnumerable<T> source)
            => Satisfy<T>(x => source.Contains(x));

        public static Parser<T, T> NoneOf<T>(IEnumerable<T> source)
            => Satisfy<T>(x => !source.Contains(x));

        public static Parser<T, T> Satisfy<T>(Func<T, bool> predicate)
            => Builder.Create<T, T>(state => (state.HasValue && predicate(state.Current))
                ? Result.Success(state.Current, state.Next)
                : Result.Fail<T, T>(state));

        public static Parser<TToken, T> Abort<TToken, T>(Func<IParsecState<TToken>, string> message)
            => Builder.Create<TToken, T>(state => { throw new ParsecException<TToken>(message(state), state); });
    }
}
