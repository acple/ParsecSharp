using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Results;

namespace ParsecSharp.Internal;

public static class Result
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Failure<TToken, TState, T>(TState state)
        where TState : IParsecState<TToken, TState>
        => new ParseError<TToken, TState, T>(state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Failure<TToken, TState, T>(Exception exception, TState state)
        where TState : IParsecState<TToken, TState>
        => new FailureWithException<TToken, TState, T>(exception, state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Failure<TToken, TState, T>(string message, TState state)
        where TState : IParsecState<TToken, TState>
        => new FailureWithMessage<TToken, TState, T>(message, state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult<TToken, T> Success<TToken, TState, T>(T value, TState state)
        where TState : IParsecState<TToken, TState>
        => new Success<TToken, TState, T>(value, state);
}
