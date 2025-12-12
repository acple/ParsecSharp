using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp;

public class TextTest
{
    [Test]
    public async Task CharTest()
    {
        // Creates a parser that matches a specified character.
        // Similar to the generic `Token` parser but optimized for char, offering better performance.

        var parser = Char('a');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task CharIgnoreCaseTest()
    {
        // Creates a parser that matches a specified character, ignoring case.

        var parser = CharIgnoreCase('A');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task StringTest()
    {
        // Creates a parser that matches a specified string.

        var parser = String("abc");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));
    }

    [Test]
    public async Task StringIgnoreCaseTest()
    {
        // Creates a parser that matches a specified string, ignoring case.

        var parser = StringIgnoreCase("abcde");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdE"));
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
    public async Task OneOfIgnoreCaseTest()
    {
        // Creates a parser that succeeds if the token is in the specified string, ignoring case.

        // Parser that matches [x-z], ignoring case.
        var parser = OneOfIgnoreCase("xyz");

        var source = "ZZZ";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('Z'));

        var source2 = "ABC";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task NoneOfIgnoreCaseTest()
    {
        // Creates a parser that succeeds if the token is not in the specified string, ignoring case.

        // Parser that matches anything except [x-z], ignoring case.
        var parser = NoneOfIgnoreCase("xyz");

        var source = "ZZZ";
        await parser.Parse(source).WillFail();

        var source2 = "ABC";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('A'));
    }
}
