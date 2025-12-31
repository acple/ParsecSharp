using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Text;

public class TextSequencePrimitivesTests
{
    [Test]
    public async Task StringTest()
    {
        // Creates a parser that matches a specified string.

        var parser = String("Hello");

        var source = "Hello world";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("Hello"));

        var source2 = "hello world";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task StringIgnoreCaseTest()
    {
        // Creates a parser that matches a specified string, ignoring case.

        var parser = StringIgnoreCase("Hello");

        var source = "Hello world";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("Hello"));

        var source2 = "hello world";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("hello"));

        var source3 = "goodbye";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task SurrogatePairTest()
    {
        // Creates a parser that matches a surrogate pair.

        var parser = SurrogatePair();

        var source = "\uD800\uDC00abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("\uD800\uDC00"));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task CrLfTest()
    {
        // Creates a parser that matches a carriage return + line feed (CRLF).

        var parser = CrLf();

        var source = "\r\nabc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        var source2 = "\nabc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task EndOfLineTest()
    {
        // Creates a parser that matches either a newline (LF) or carriage return + line feed (CRLF).

        var parser = EndOfLine();

        var source = "\nabc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        var source2 = "\r\nabc";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        var source3 = "abc";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task SpacesTest()
    {
        // Creates a parser that skips zero or more Unicode whitespace characters.

        var parser = Spaces();

        var source = "   abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        await parser.Right(Any()).Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "abc";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task Spaces1Test()
    {
        // Creates a parser that skips one or more Unicode whitespace characters.

        var parser = Spaces1();

        var source = "   abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }
}
