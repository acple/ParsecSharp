using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyRec<TToken, T>(Parser<TToken, T> parser, List<T> list)
            => parser.Next(x => { list.Add(x); return ManyRec(parser, list); }, list);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyTillRec<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator, List<T> list)
            => terminator.Next(list.Const, parser.Bind(x => { list.Add(x); return ManyTillRec(parser, terminator, list); }).Const);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, T> ChainRec<TToken, T>(Func<T, Parser<TToken, T>> rest, T value)
            => rest(value).Next(x => ChainRec(rest, x), value);
    }
}
