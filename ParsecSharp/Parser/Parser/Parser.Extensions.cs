using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Parser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Parse<TToken, T>(this IParser<TToken, T> parser, IEnumerable<TToken> source)
        => parser.Parse(EnumerableStream.Create(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Parse<TToken, T>(this IParser<TToken, T> parser, IReadOnlyList<TToken> source)
        => parser.Parse(ArrayStream.Create(source));
}
