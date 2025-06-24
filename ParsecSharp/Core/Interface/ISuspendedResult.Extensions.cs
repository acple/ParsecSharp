using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static class SuspendedResultExtensions
{
    extension<TToken, T>(ISuspendedResult<TToken, T> source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out IResult<TToken, T> result, out ISuspendedState<TToken> rest)
        {
            result = source.Result;
            rest = source.Rest;
        }
    }
}
