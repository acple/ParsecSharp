using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyRec<TToken, T>(Parser<TToken, T> parser, IEnumerable<T> result)
            => parser.Next(x => ManyRec(parser, result.Append(x)), result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, IEnumerable<T>> ManyTillRec<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator, IEnumerable<T> result)
            => terminator.Next(result.Const, parser.Bind(x => ManyTillRec(parser, terminator, result.Append(x))).Const);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Parser<TToken, T> ChainRec<TToken, T>(Func<T, Parser<TToken, T>> chain, T value)
            => chain(value).Next(x => ChainRec(chain, x), value);
    }
}
