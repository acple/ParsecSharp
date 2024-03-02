using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<char, T> Fix<T>(Func<Parser<char, T>, Parser<char, T>> function)
            => Parser.Fix(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TParameter, Parser<char, T>> Fix<TParameter, T>(Func<Func<TParameter, Parser<char, T>>, TParameter, Parser<char, T>> function)
            => Parser.Fix(function);
    }
}
