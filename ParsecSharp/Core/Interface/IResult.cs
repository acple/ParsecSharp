using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public interface IResult<TToken, out T>
    {
        T Value { get; }

        TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success);

        IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function);

        string ToString();

        internal ISuspendedResult<TToken, T> Suspend();
    }

    public interface IFailure<TToken, out T> : IResult<TToken, T>
    {
        IParsecState<TToken> State { get; }

        ParsecException Exception { get; }

        string Message { get; }

        IResult<TToken, TResult> Convert<TResult>();
    }

    public interface ISuccess<TToken, out T> : IResult<TToken, T>
    {
        internal IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont);
    }

    public static class ResultExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CaseOf<TToken, T>(this IResult<TToken, T> result, Action<IFailure<TToken, T>> failure, Action<ISuccess<TToken, T>> success)
            => result.CaseOf<object?>(
                result => { failure(result); return default; },
                result => { success(result); return default; });
    }
}
