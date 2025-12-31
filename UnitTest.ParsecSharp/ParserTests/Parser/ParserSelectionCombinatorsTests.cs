using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserSelectionCombinatorsTests
{
    [Test]
    public async Task ChoiceTest()
    {
        // Creates a parser that applies parsers from the beginning and returns the result of the first one that succeeds.
        // If all fail, the last failure is returned as the overall failure.

        // Parser that matches 'c', 'b', or 'a'.
        var parser = Choice(Char('c'), Char('b'), Char('a'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }
}
