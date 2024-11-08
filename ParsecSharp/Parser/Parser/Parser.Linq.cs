using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IParser<TToken, TResult> Select<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> selector)
            => parser.Map(selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IParser<TToken, TResult> SelectMany<TToken, T, TIntermediate, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TIntermediate>> selector, Func<T, TIntermediate, TResult> projector)
            => parser.Bind(x => selector(x).Map(y => projector(x, y)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IParser<TToken, T> Where<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate)
            => parser.Guard(predicate);
    }
}
