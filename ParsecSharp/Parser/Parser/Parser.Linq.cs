using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Parser<TToken, TResult> Select<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> selector)
            => parser.Map(selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Parser<TToken, TResult> SelectMany<TToken, T, TTemp, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TTemp>> selector, Func<T, TTemp, TResult> projector)
            => parser.Bind(x => selector(x).Map(y => projector(x, y)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Parser<TToken, T> Where<TToken, T>(this Parser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Guard(predicate);
    }
}
