using System;
using System.Runtime.CompilerServices;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CaseOf<TToken, T>(this Result<TToken, T> result, Action<Fail<TToken, T>> fail, Action<Success<TToken, T>> success)
            => result.CaseOf<object>(
                x => { fail(x); return default; },
                x => { success(x); return default; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action)
            => parser.Map(x => { action(x); return x; });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action, Action<IParsecState<TToken>> onFail)
            => parser.ModifyResult(
                (_, fail) => { onFail(fail.State); return fail; },
                (_, success) => { action(success.Value); return success; });
    }
}
