using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public partial interface ISuspendedResult<TToken, out T> : IDisposable
    {
        IResult<TToken, T> Result { get; }

        ISuspendedState<TToken> Rest { get; }
    }

    public partial interface ISuspendedState<TToken> : IDisposable
    {
        IDisposable? InnerResource { get; }

        ISuspendedResult<TToken, T> Continue<T>(IParser<TToken, T> parser);
    }

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
