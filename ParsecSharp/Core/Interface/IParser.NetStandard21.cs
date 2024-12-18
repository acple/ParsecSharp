#if NET || NETSTANDARD2_1_OR_GREATER
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp;

public partial interface IParser<TToken, out T>
{
    static IParser<TToken, T> operator |(IParser<TToken, T> first, IParser<TToken, T> second)
        => new Alternative<TToken, T>(first, second);
}
#endif
