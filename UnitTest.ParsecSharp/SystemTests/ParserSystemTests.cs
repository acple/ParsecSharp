using System;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.SystemTests;

public class ParserSystemTests
{
    [Test]
    public async Task ExceptionTest()
    {
        // If an exception occurs during processing, the parser immediately stops and returns a `Failure` containing the exception.
        // Recovery is not performed for failures due to exceptions. There is no means of recovery.

        // Parser that attempts to return the result of `ToString` on null, and returns "success" if it fails.
        var parser = Pure(null as object).Map(x => x!.ToString()).Or(Pure("success"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Exception.InnerException).IsTypeOf<NullReferenceException>());
    }

    [Test]
    public async Task ParsePartiallyTest()
    {
        // Provides an execution plan that allows you to continue processing without disposing the stream after parsing is complete.

        // Parser that consumes 3 characters.
        var parser = Any().Repeat(3).AsString();

        using var source = StringStream.Create("abcdEFGH");

        var (result, rest) = parser.ParsePartially(source);
        await result.WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

        var (result2, rest2) = parser.ParsePartially(rest);
        await result2.WillSucceed(async value => await Assert.That(value).IsEqualTo("dEF"));

        var (result3, rest3) = parser.ParsePartially(rest2);
        await result3.WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Unexpected '<EndOfStream>'")); // Fails because it reached the end

        // Note that the state at the point of failure is returned.
        await EndOfInput().Parse(rest3).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }
}
