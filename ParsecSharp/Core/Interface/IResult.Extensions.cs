using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static class ResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CaseOf<TToken, T>(this IResult<TToken, T> result, Action<IFailure<TToken, T>> failure, Action<ISuccess<TToken, T>> success)
        => result.CaseOf<object?>(
            result => { failure(result); return default; },
            result => { success(result); return default; });
}
