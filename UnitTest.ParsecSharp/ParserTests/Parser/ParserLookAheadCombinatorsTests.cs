using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserLookAheadCombinatorsTests
{
    [Test]
    public async Task NotTest()
    {
        // Creates a parser that succeeds if parser fails.
        // This parser does not consume input.

        // Parser that succeeds if the token is not `Lower`.
        var parser = Not(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail();

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task LookAheadTest()
    {
        // Creates a parser that performs a parse with parser without consuming input.

        // Parser that matches `Any` then `Letter` without consuming input, then matches `Any` and concatenates the results.
        var parser = LookAhead(Any() + Letter()) + Any();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("aba"));

        var source2 = "123456";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): At LookAhead -> Parser Failure (Line: 1, Column: 2): Unexpected '2<0x32>'"));
    }
}
