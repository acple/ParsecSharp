using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<T, T> Any<T>()
            => Builder.Create<T, T>(state => (state.HasValue)
                ? Result.Success(state.Current, state.Next)
                : Result.Fail<T, T>(state));

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

        public static Parser<TToken, Unit> Error<TToken>()
            => Builder.Create<TToken, Unit>(state => Result.Fail<TToken, Unit>(state));

        public static Parser<TToken, Unit> Error<TToken>(string message)
            => Builder.Create<TToken, Unit>(state => Result.FailWithMessage<TToken, Unit>(message, state));
    }
}
