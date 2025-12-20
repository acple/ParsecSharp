using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class TextTransformationExtensionsTests
{
    [Test]
    public async Task AsStringTest()
    {
        // Converts a parser that returns a single char or IEnumerable<char> to a parser that returns a string.

        // Single char to string.
        var parser = Char('a').AsString();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("a"));

        // Collection of chars to string.
        var parser2 = Many1(AsciiLetter()).AsString();

        var source2 = "hello123";
        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("hello"));

        // Converts 3 chars to string.
        var parser3 = AsciiLetter().Repeat(3).AsString();

        var source3 = "abcdef";
        await parser3.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

        var source4 = "ab";
        await parser3.Parse(source4).WillFail();
    }

    [Test]
    public async Task ToIntTest()
    {
        // A combinator that converts the result string to an integer.

        var source = "1234abcd";

        // Parser that matches [0 - 9] and converts to int.
        var parser = Many1(DecDigit()).ToInt();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1234));

        // Fails if the value is non-numeric.
        var parser2 = Many1(Any()).ToInt();
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected digits but was '1234abcd'"));

        // Fails if the value exceeds 32-bit range.
        var source2 = "1234567890123456";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected digits but was '1234567890123456'"));
    }

    [Test]
    public async Task ToLongTest()
    {
        // A combinator that converts the result string to a long integer.

        var source = "1234abcd";

        // Parser that matches [0 - 9] and converts to long.
        var parser = Many1(DecDigit()).ToLong();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1234L));

        // Fails if the value is non-numeric.
        var parser2 = Many1(Any()).ToLong();
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected digits but was '1234abcd'"));

        // Can convert values up to 64-bit range.
        var source2 = "1234567890123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(1234567890123456L));
    }

    [Test]
    public async Task ToDoubleTest()
    {
        // A combinator that converts the result string to a double.

        var source = "1234.5678abcd";

        // Parser that matches [0 - 9] / '.' and converts to double.
        var parser = Many1(DecDigit() | Char('.')).ToDouble();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1234.5678));

        // Fails if the value is non-numeric.
        var parser2 = Many1(Any()).ToDouble();
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected number but was '1234.5678abcd'"));

        // Supports strings that can be converted using `double.Parse`.
        var source2 = "1.234567890123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(1.234567890123456));
    }

    [Test]
    public async Task JoinTest()
    {
        // Concatenates a collection of strings into a single string.
        // Optionally takes a separator string.

        // Join without separator - concatenates strings directly.
        var parser = Many1(AsciiLetter().Repeat(2).AsString()).Join();

        var source = "abcdefgh";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdefgh"));

        // Join with separator - inserts separator between strings.
        var parser2 = AsciiLetter().Repeat(2).AsString().SeparatedBy1(Char(',')).Join("-");

        var source2 = "ab,cd,ef";
        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab-cd-ef"));

        // Useful for building CSV-like formats.
        var parser3 = Many(AsciiLetter().Repeat(2).AsString()).Join(",");

        var source3 = "abcdefg";
        await parser3.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab,cd,ef"));

        // Works with empty collections.
        await parser3.Parse(string.Empty).WillSucceed(async value => await Assert.That(value).IsEqualTo(""));
    }
}
