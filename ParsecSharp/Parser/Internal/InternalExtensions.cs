using System.Collections.Generic;

namespace Parsec.Internal
{
    public static class InternalExtensions
    {
        public static Result<TToken, T> Parse<TToken, T>(this Parser<TToken, T> parser, IEnumerable<TToken> source)
            => parser.Parse(new EnumerableStream<TToken>(source));
    }
}
