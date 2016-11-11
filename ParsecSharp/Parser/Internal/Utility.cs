namespace Parsec.Internal
{
    internal static class Utility
    {
        internal static IParsecStateStream<T> Advance<T>(this IParsecStateStream<T> state, int count)
            => (1 <= count && state.HasValue)
                ? state.Next.Advance(count - 1)
                : state;
    }
}
