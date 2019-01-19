using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Parsec
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Parse<TToken, T>(this Parser<TToken, T> parser, IEnumerable<TToken> source)
            => parser.Parse(new EnumerableStream<TToken>(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TToken, T> Parse<TToken, T>(this Parser<TToken, T> parser, IReadOnlyList<TToken> source)
            => parser.Parse(new ArrayStream<TToken>(source));
    }
}
