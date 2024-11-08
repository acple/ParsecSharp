using System;

namespace ParsecSharp
{
    public partial interface IParser<TToken, out T>
    {
        IResult<TToken, T> Parse<TState>(TState source)
            where TState : IParsecState<TToken, TState>;

        IResult<TToken, T> Parse(ISuspendedState<TToken> suspended);

        ISuspendedResult<TToken, T> ParsePartially<TState>(TState source)
            where TState : IParsecState<TToken, TState>;

        ISuspendedResult<TToken, T> ParsePartially(ISuspendedState<TToken> source);

        internal IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            where TState : IParsecState<TToken, TState>;
    }
}
