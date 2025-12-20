using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserValidationExtensionsTests
{
    [Test]
    public async Task EndTest()
    {
        // A combinator that ensures that parser consumes the input until the end.
        // Fails if it has not reached the end.

        var source = "abcdEFGH";

        // Parser that matches 1 or more lowercase letters and must consume all input at that point.
        var parser = Many1(Lower()).End();
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected '<EndOfStream>' but was 'E<0x45>'"));

        // Parser that matches 1 or more lowercase or uppercase letters and must consume all input at that point.
        var parser2 = Many1(Lower() | Upper()).End();
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcdEFGH"));
    }

    [Test]
    public async Task WithConsumeTest()
    {
        // Creates a parser that treats it as a failure if parser succeeds without consuming input.
        // Used when passing a parser that may not consume input, such as `Many`.

        // Parser that avoids infinite loops that occur when `Letter` does not match.
        var parser = Many1(Many(Letter()).WithConsume().AsString());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["abcdEFGH"]));

        var source2 = "123456";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): A parser did not consume any input"));
    }

    [Test]
    public async Task WithMessageTest()
    {
        // Rewrites the error message when parsing fails.

        var parser = Lower().Repeat(6)
            .WithMessage(failure => $"MessageTest Current: '{failure.State.Current}', original message: {failure.Message}");

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 5): MessageTest Current: 'E', original message: Unexpected 'E<0x45>'"));

        var source2 = "abcdefgh";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcdef"));

        var source3 = "123456";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task AbortWhenFailTest()
    {
        // Aborts the parsing process when parsing fails.

        var parser = Many(Lower().AbortWhenFail(failure => $"Fatal Error! '{failure.State.Current}' is not a lower char!")).AsString()
            .Or(Pure("recovery"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 5): Fatal Error! 'E' is not a lower char!"));
    }

    [Test]
    public async Task AbortIfEnteredTest()
    {
        // Aborts the parsing process if the parser fails after consuming input.
        // Achieves early exit on failure like LL(k) parsers.

        var parser = Sequence(Char('1'), Char('2'), Char('3'), Char('4')).AsString().AbortIfEntered(_ => "abort1234")
            .Or(Pure("recovery"));

        var source = "123456";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));

        var source2 = "abcdEFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("recovery"));

        var source3 = "123,456,789";
        await parser.Parse(source3).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("abort1234"));
    }
}
