using System;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static void CaseOf<TToken, T>(this Result<TToken, T> result, Action<Fail<TToken, T>> fail, Action<Success<TToken, T>> success)
            => result.CaseOf<object>(
                x => { fail(x); return null; },
                x => { success(x); return null; });

        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> success)
            => parser.Do(success, () => { });

        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action fail)
            => parser.Do(_ => { }, fail);

        public static Parser<TToken, T> Do<TToken, T>(this Parser<TToken, T> parser, Action<T> success, Action fail)
            => parser.ModifyResult(
                (_, result) => { fail(); return result; },
                (_, result) => { success(result.Value); return result; });
    }
}
