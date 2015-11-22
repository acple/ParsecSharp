using System.Collections.Generic;

namespace Parsec.Internal
{
    public static class InternalExtensions
    {
        public static Result<T, TResult> Parse<T, TResult>(this Parser<T, TResult> parser, IEnumerable<T> source)
            => parser.Parse(new EnumerableStream<T>(source));
    }
}
