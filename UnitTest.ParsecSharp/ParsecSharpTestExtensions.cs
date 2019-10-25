using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;

namespace UnitTest.ParsecSharp
{
    internal static class ParsecSharpTestExtensions
    {
        public static void WillFail<TToken, T>(this Result<TToken, T> result)
            => result.WillFail(_ => { /* expect to fail */ });

        public static void WillFail<TToken, T>(this Result<TToken, T> result, Action<Fail<TToken, T>> assert)
            => result.CaseOf(
                fail => assert(fail),
                success => Assert.Fail(success.ToString()));

        public static void WillSucceed<TToken, T>(this Result<TToken, T> result, Action<T> assert)
            => result.CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => assert(success.Value));
    }
}
