using System;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, TResult> Bind<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> function)
            => new Bind<TToken, T, TResult>(parser, function);

        public static Parser<TToken, T> Alternative<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => new Alternative<TToken, T>(parser, next);

        public static Parser<TToken, TResult> FMap<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> function)
            => parser.Bind(x => Return<TToken, TResult>(function(x)));

        public static Parser<TToken, T> Guard<TToken, T>(this Parser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Bind(x => (predicate(x)) ? Return<TToken, T>(x) : Fail<TToken, T>());
    }
}
