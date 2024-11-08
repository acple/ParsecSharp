using System;
using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<char, T> Fix<T>(Func<IParser<char, T>, IParser<char, T>> function)
            => Parser.Fix(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TParameter, IParser<char, T>> Fix<TParameter, T>(Func<Func<TParameter, IParser<char, T>>, TParameter, IParser<char, T>> function)
            => Parser.Fix(function);
    }
}
