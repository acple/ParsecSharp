namespace Parsec.Internal
{
    internal static partial class Utility
    {
        public static IParsecStateStream<TToken> Advance<TToken>(this IParsecStateStream<TToken> state, int count)
            => (1 <= count && state.HasValue)
                ? state.Next.Advance(count - 1)
                : state;
    }
}
