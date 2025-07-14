using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Parser
{
    extension<TToken, T>(IParser<TToken, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse(IEnumerable<TToken> source)
            => parser.Parse(EnumerableStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse(IReadOnlyList<TToken> source)
            => parser.Parse(ArrayStream.Create(source));
    }
}
