using System;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, T> Return<TToken, T>(T value)
            => Builder.Create<TToken, T>(state => Result.Success(value, state));

        public static Parser<TToken, T> Return<TToken, T>(Func<T> valueFactory)
            => Builder.Create<TToken, T>(state => Result.Success(valueFactory(), state));

        public static Parser<TToken, TResult> Bind<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> function)
            => Builder.Create<TToken, TResult>(state => parser.Run(state).Next(function));

        public static Parser<TToken, TResult> Bind<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> onNext, Func<Parser<TToken, TResult>> onFail)
            => Builder.Create<TToken, TResult>(state => parser.Run(state).CaseOf(
                fail => onFail().Run(state),
                success => success.Next(onNext)));

        public static Parser<TToken, TResult> FMap<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function)
            => parser.Bind(value => Return<TToken, TResult>(function(value)));

        public static Parser<TToken, T> Alternative<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => Builder.Create<TToken, T>(state => parser.Run(state).CaseOf(
                fail => next.Run(state),
                success => success));
    }
}
