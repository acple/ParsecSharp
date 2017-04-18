using System;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static void CaseOf<TToken, T>(this Result<TToken, T> result, Action<Fail<TToken, T>> fail, Action<Success<TToken, T>> success)
            => result.CaseOf(
                x => { fail(x); return default(object); },
                x => { success(x); return default(object); });

        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action)
            => parser.Map(x => { action(x); return x; });

        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> action, Action<IParsecState<TToken>> failed)
            => parser.ModifyResult(
                (_, fail) => { failed(fail.State); return fail; },
                (_, success) => { action(success.Value); return success; });
    }
}
