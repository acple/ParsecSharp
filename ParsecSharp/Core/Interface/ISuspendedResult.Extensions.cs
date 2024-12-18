using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static class SuspendedResultExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct<TToken, T>(this ISuspendedResult<TToken, T> source, out IResult<TToken, T> result, out ISuspendedState<TToken> rest)
        {
            result = source.Result;
            rest = source.Rest;
        }
    }
}
