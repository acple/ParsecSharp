using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;
using ParsecSharp.Internal;

namespace ParsecSharp;

public static class ParsecState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TState> AsEnumerable<TToken, TState>(TState stream)
        where TState : IParsecState<TToken, TState>
        => new ParsecStateEnumerable<TToken, TState>(stream);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecStateStream<TToken, LinearPosition<TToken>> Tokenize<TInput, TState, TToken>(TState source, IParser<TInput, TToken> parser)
        where TState : IParsecState<TInput, TState>
        => Tokenize(source, parser, LinearPosition<TToken>.Initial);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecStateStream<TToken, TPosition> Tokenize<TInput, TState, TToken, TPosition>(TState source, IParser<TInput, TToken> parser, TPosition position)
        where TState : IParsecState<TInput, TState>
        where TPosition : IPosition<TToken, TPosition>
        => Tokenize(source.InnerResource, parser.ParsePartially(source), parser, position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ParsecStateStream<TToken, TPosition> Tokenize<TInput, TToken, TPosition>(IDisposable? resource, ISuspendedResult<TInput, TToken> state, IParser<TInput, TToken> parser, TPosition position)
        where TPosition : IPosition<TToken, TPosition>
        => state.Result.CaseOf<ParsecStateStream<TToken, TPosition>>(
            failure => new(position, resource),
            success => new(success.Value, position, resource, () => Tokenize(state.Rest.InnerResource, state.Rest.Continue(parser), parser, position.Next(success.Value))));
}
