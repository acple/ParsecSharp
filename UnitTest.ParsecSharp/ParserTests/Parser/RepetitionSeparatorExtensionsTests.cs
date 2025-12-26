using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class RepetitionSeparatorExtensionsTests
{
    [Test]
    public async Task SeparatedByTest()
    {
        // Creates a parser that matches parser repeated 0 or more times separated by separator.
        // The result of matching separator is discarded.

        // [ 1*Number *( "," 1*Number ) ]
        var parser = Many1(Number()).AsString().SeparatedBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123456"]));

        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task SeparatedBy1Test()
    {
        // Creates a parser that matches parser repeated 1 or more times separated by separator.

        // 1*Number *( "," 1*Number )
        var parser = Many1(Number()).AsString().SeparatedBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123456"]));

        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task EndByTest()
    {
        // Creates a parser that matches parser repeated 0 or more times with separator at the end.

        // *( 1*Number "," )
        var parser = Many1(Number()).AsString().EndBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task EndBy1Test()
    {
        // Creates a parser that matches parser repeated 1 or more times with separator at the end.

        // 1*( 1*Number "," )
        var parser = Many1(Number()).AsString().EndBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456"]));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SeparatedOrEndByTest()
    {
        // Creates a parser that behaves as either `SeparatedBy` or `EndBy`.

        // [ 1*Number *( "," 1*Number ) [ "," ] ]
        var parser = Many1(Number()).AsString().SeparatedOrEndBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123456"]));

        var source3 = "123,456,789" + ",";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));
    }

    [Test]
    public async Task SeparatedOrEndBy1Test()
    {
        // Creates a parser that behaves as either `SeparatedBy1` or `EndBy1`.

        // 1*Number *( "," 1*Number ) [ "," ]
        var parser = Many1(Number()).AsString().SeparatedOrEndBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123456"]));

        var source3 = "123,456,789" + ",";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["123", "456", "789"]));
    }
}
