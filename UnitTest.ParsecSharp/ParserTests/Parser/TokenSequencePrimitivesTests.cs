using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class TokenSequencePrimitivesTests
{
    [Test]
    public async Task TakeTest()
    {
        // Creates a parser that reads the specified number of tokens and returns the result as a sequence.

        // Parser that reads 3 tokens.
        var parser = Take(3);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abc"));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("123"));

        // If the specified number of tokens exceeds the remaining input, the parser fails.
        var parser2 = Take(9);
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));

        // If 0 is specified, the parser succeeds without consuming input.
        var parser3 = Take(0);
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEmpty());

        // If a value less than 0 is specified, the parser always fails.
        var parser4 = Take(-1);
        await parser4.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));
    }

    [Test]
    public async Task TakeWhileTest()
    {
        // Creates a parser that continues to read input as long as the given condition is met.

        // Parser that continues to read input as long as the token is lowercase letter.
        var parser = TakeWhile(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task TakeWhile1Test()
    {
        // Creates a parser that continues to read input as long as the given condition is met.
        // If no match is found, the parser fails.

        // Parser that continues to read input as long as the token is lowercase letter.
        var parser = TakeWhile1(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SkipTest()
    {
        // Creates a parser that skips the specified number of tokens.

        var source = "abcdEFGH";

        // Parser that skips 3 tokens and then returns the next token.
        var parser = Skip(3).Right(Any());

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('d'));

        var parser2 = Skip(8).Right(EndOfInput());
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        // If the specified number of tokens cannot be skipped, the parser fails.
        var parser3 = Skip(9);
        await parser3.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));

        // If 0 is specified, the parser succeeds without consuming input.
        var parser4 = Skip(0);
        await parser4.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        // If a value less than 0 is specified, the parser always fails.
        var parser5 = Skip(-1);
        await parser5.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));
    }

    [Test]
    public async Task SkipWhileTest()
    {
        // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
        // Works the same as `TakeWhile` but does not collect the result, making it more efficient.

        // Parser that continues to consume input as long as the token is lowercase letter.
        var parser = SkipWhile(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task SkipWhile1Test()
    {
        // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
        // If it cannot skip at least one token, it fails.

        // Parser that continues to consume input as long as the token is lowercase letter.
        var parser = SkipWhile1(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }
}
