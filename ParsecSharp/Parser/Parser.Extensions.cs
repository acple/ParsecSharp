using System.Collections.Generic;

namespace Parsec
{
    public static partial class Parser
    {
        public static Result<TToken, T> Parse<TToken, T>(this Parser<TToken, T> parser, IEnumerable<TToken> source)
            => parser.Parse(new EnumerableStream<TToken>(source));

        public static Result<TToken, T> Parse<TToken, T>(this Parser<TToken, T> parser, IReadOnlyList<TToken> source)
            => parser.Parse(new ArrayStream<TToken>(source));
    }
}
