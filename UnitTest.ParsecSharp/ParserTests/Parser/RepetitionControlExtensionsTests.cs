using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class RepetitionControlExtensionsTests
{
    [Test]
    public async Task RepeatTest()
    {
        // Creates a parser that matches parser count times and returns the result as a sequence.

        // 2*( 3*Any )
        var parser = Any().Repeat(3).AsString().Repeat(2);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["abc", "dEF"]));
    }
}
