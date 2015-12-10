using System;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, TResult> Select<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> selector)
            => parser.FMap(selector);

        public static Parser<TToken, TResult> SelectMany<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> selector)
            => parser.Bind(selector);

        public static Parser<TToken, TResult> SelectMany<TToken, T, TTemp, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TTemp>> selector, Func<T, TTemp, TResult> resultSelector)
            => parser.Bind(x => selector(x).FMap(y => resultSelector(x, y)));

        public static Parser<TToken, T> Where<TToken, T>(this Parser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Guard(predicate);
    }
}
