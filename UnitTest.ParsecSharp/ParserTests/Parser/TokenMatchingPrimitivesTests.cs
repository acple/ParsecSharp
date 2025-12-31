using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class TokenMatchingPrimitivesTests
{
    [Test]
    public async Task AnyTest()
    {
        // Creates a parser that matches any token.
        // This parser only fails if the input is at the end.

        var parser = Any();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task TokenTest()
    {
        // Creates a parser that matches a specified token.
        // Uses `EqualityComparer<T>.Default` for equality comparison.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that matches the token 'a'.
        var parser = Token('a');

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task OneOfTest()
    {
        // Creates a parser that succeeds if the token is included in the specified sequence.

        // Parser that succeeds if the token is one of '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e'.
        var parser = OneOf("6789abcde");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();

        // Overload that takes `params IEnumerable<char>`.
        var parser2 = OneOf('6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e');
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser2.Parse(source2).WillFail();
    }

    [Test]
    public async Task NoneOfTest()
    {
        // Creates a parser that succeeds if the token is not included in the specified sequence.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that succeeds if the token is not one of 'd', 'c', 'b', 'a', '9', '8', '7'.
        var parser = NoneOf("dcba987");

        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        // Overload that takes `params IEnumerable<char>`.
        var parser2 = NoneOf('d', 'c', 'b', 'a', '9', '8', '7');

        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));
    }

    [Test]
    public async Task SatisfyTest()
    {
        // Creates a parser that takes one input and succeeds if the condition is met.
        // Can be used to construct all parsers that consume input.

        var source = "abcdEFGH";

        // Parser that matches 'a'.
        var parser = Satisfy(x => x == 'a'); // == Char('a');
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        // Parser that matches any of 'a', 'b', 'c'.
        var parser2 = Satisfy("abc".Contains); // == OneOf("abc");
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }
}
