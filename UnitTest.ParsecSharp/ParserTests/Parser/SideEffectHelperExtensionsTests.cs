using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class SideEffectHelperExtensionsTests
{
    [Test]
    public async Task DoTest()
    {
        // Executes the defined action when parsing executes.
        // Can specify actions to be executed on success, or on both success and failure.

        var source = "abcdEFGH";

        // Increases the value of count by 1 when parsing `Lower` succeeds.
        var count = 0;
        var parser = Many(Lower().Do(_ => count++));

        _ = parser.Parse(source);
        _ = await Assert.That(count).IsEqualTo(4);
        _ = parser.Parse(source);
        _ = await Assert.That(count).IsEqualTo(8);

        // Increases the value of success by 1 when parsing `Lower` succeeds, and increases the value of failure by 1 when parsing fails.
        // Connects `Any`, so it parses the source to the end.
        var success = 0;
        var failure = 0;
        var parser2 = Many(Lower().Do(_ => success++, _ => failure++).Or(Any()));

        _ = parser2.Parse(source);
        _ = await Assert.That(success).IsEqualTo(4);
        _ = await Assert.That(failure).IsEqualTo(5);
    }
}
