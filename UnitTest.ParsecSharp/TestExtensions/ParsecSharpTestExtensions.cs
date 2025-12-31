using System;
using System.Threading.Tasks;
using ParsecSharp;

namespace UnitTest.ParsecSharp;

internal static class ParsecSharpTestExtensions
{
    extension<TToken, T>(IResult<TToken, T> result)
    {
        public Task WillFail()
            => result.WillFail(_ => Task.CompletedTask);

        public Task WillFail(Func<IFailure<TToken, T>, Task> assert)
            => result.CaseOf(
                failure: assert,
                static async success => Assert.Fail($"Expected failure but got success: {success.ToString()}"));

        public Task WillSucceed(Func<T, Task> assert)
            => result.CaseOf(
                static async failure => Assert.Fail($"Expected success but got failure: {failure.ToString()}"),
                success => assert(success.Value));
    }
}
