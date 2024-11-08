using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Abort<TToken, T>(Exception exception) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => Result.Failure<TToken, TState, TResult>(exception, state);
    }
}
