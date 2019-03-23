using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyRec<TToken, T>(Parser<TToken, T> parser, List<T> list)
            => parser.Next(x => { list.Add(x); return ManyRec(parser, list); }, list);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyTillRec<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator, List<T> list)
            => terminator.Map(_ => list.AsEnumerable())
                .Alternative(parser.Bind(x => { list.Add(x); return ManyTillRec(parser, terminator, list); }));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, T> ChainRec<TToken, T>(Func<T, Parser<TToken, T>> rest, T value)
            => rest(value).Next(x => ChainRec(rest, x), value);
    }
}
