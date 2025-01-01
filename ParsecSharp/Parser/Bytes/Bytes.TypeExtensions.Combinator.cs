using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Bytes
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Fix<T>(Func<IParser<byte, T>, IParser<byte, T>> function)
        => Parser.Fix(function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<TParameter, IParser<byte, T>> Fix<TParameter, T>(Func<Func<TParameter, IParser<byte, T>>, TParameter, IParser<byte, T>> function)
        => Parser.Fix(function);
}
