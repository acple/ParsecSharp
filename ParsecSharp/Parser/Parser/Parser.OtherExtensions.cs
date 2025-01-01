using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp;

public static partial class Parser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Do<TToken, T>(this IParser<TToken, T> parser, Action<T> action)
        => parser.Map(x => { action(x); return x; });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Do<TToken, T>(this IParser<TToken, T> parser, Action<T> action, Action<IFailure<TToken, T>> failure)
        => new Do<TToken, T>(parser, failure, action);
}
