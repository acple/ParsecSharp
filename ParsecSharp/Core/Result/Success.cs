using System;

namespace ParsecSharp.Internal
{
    public abstract class Success<TToken, T>(T value) : ISuccess<TToken, T>
    {
        public T Value => value;

        public TResult CaseOf<TResult>(Func<IFailure<TToken, T>, TResult> failure, Func<ISuccess<TToken, T>, TResult> success)
            => success(this);

        public abstract IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function);

        public abstract IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont);

        public abstract ISuspendedResult<TToken, T> Suspend();

        public sealed override string ToString()
            => value?.ToString() ?? string.Empty;
    }
}
