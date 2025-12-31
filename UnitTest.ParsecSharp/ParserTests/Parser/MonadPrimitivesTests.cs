using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class MonadPrimitivesTests
{
    [Test]
    public async Task PureTest()
    {
        // Creates a parser that returns a success result.
        // Used to inject arbitrary values into the parser.
        // This parser does not consume input.

        var parser = Pure("success!");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("success!"));

        // Delays the generation of the value until the parser is executed.
        var parser2 = Pure(_ => Unit.Instance);
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task FailTest()
    {
        // Creates a parser that returns a failure result.
        // This parser does not consume input.

        var source = "abcdEFGH";

        var parser = Fail<Unit>();
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        // Overload that allows specifying an error message.
        var parser2 = Fail<Unit>("errormessagetest");
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): errormessagetest"));

        // Can handle the state at the time of parse failure.
        var parser3 = Fail<Unit>(state => $"errormessagetest, current state: '{state}'");
        await parser3.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): errormessagetest, current state: 'a<0x61>'"));
    }

    [Test]
    public async Task AbortTest()
    {
        // Creates a parser that aborts the parsing process when executed.
        // Usually not used directly. Use the `AbortIfEntered` or `AbortWhenFail` combinator.

        // Matches `Abort` or `Any`, but the parsing process ends when `Abort` is evaluated.
        var parser = Abort<char>(_ => "aborted").Or(Any());

        var source = "123456";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("aborted"));
    }

    [Test]
    public async Task GetPositionTest()
    {
        // Creates a parser that retrieves the position of the parse location.
        // This parser does not consume input.

        // Parser that matches `Any` 3 times and then returns the position at that point.
        var parser = Any().Repeat(3).Right(GetPosition());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value.Column).IsEqualTo(4));
    }
}
