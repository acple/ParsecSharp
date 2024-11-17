using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IParser<TToken, IReadOnlyCollection<T>> ManyRec<TToken, T>(IParser<TToken, T> parser, IReadOnlyCollection<T> result)
            => parser.Next(x => ManyRec(parser, result.Append(x)), result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IParser<TToken, IReadOnlyCollection<T>> ManyTillRec<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator, IReadOnlyCollection<T> result)
            => terminator.Next(result.Const, parser.Bind(x => ManyTillRec(parser, terminator, result.Append(x))).Const);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IParser<TToken, T> ChainRec<TToken, T>(Func<T, IParser<TToken, T>> chain, T value)
            => chain(value).Next(x => ChainRec(chain, x), value);
    }
}
