using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static class ResultExtensions
{
    extension<TToken, T>(IResult<TToken, T> result)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CaseOf(Action<IFailure<TToken, T>> failure, Action<ISuccess<TToken, T>> success)
            => result.CaseOf<object?>(
                result => { failure(result); return default; },
                result => { success(result); return default; });
    }
}
