using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, T> Fix<T>(Func<Parser<byte, T>, Parser<byte, T>> function)
            => Parser.Fix(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TParameter, Parser<byte, T>> Fix<TParameter, T>(Func<Func<TParameter, Parser<byte, T>>, TParameter, Parser<byte, T>> function)
            => Parser.Fix(function);
    }
}
