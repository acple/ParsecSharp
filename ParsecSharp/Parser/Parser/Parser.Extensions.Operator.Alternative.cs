using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Parser
{
    extension<TToken, T>(IParser<TToken, T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> operator |(IParser<TToken, T> first, IParser<TToken, T> second)
            => first.Alternative(second);
    }
}
