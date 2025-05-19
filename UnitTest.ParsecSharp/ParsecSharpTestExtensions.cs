using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;

namespace UnitTest.ParsecSharp;

internal static class ParsecSharpTestExtensions
{
    extension<TToken, T>(IResult<TToken, T> result)
    {
        public void WillFail()
            => result.WillFail(_ => { /* expect to fail */ });

        public void WillFail(Action<IFailure<TToken, T>> assert)
            => result.CaseOf(
                failure => assert(failure),
                success => Assert.Fail(success.ToString()));

        public void WillSucceed(Action<T> assert)
            => result.CaseOf(
                failure => Assert.Fail(failure.ToString()),
                success => assert(success.Value));
    }
}
