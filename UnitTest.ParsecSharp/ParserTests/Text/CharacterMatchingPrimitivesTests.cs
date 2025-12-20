using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Text;

public class CharacterMatchingPrimitivesTests
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
    public async Task OneOfTest()
    {
        // Creates a parser that succeeds if the token is in the specified string.

        // Parser that matches [x-z].
        var parser = OneOf("xyz");

        var source = "zzz";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('z'));

        var source2 = "ABC";
        await parser.Parse(source2).WillFail();
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
    public async Task NoneOfTest()
    {
        // Creates a parser that succeeds if the token is not in the specified string.

        // Parser that matches anything except [x-z].
        var parser = NoneOf("xyz");

        var source = "zzz";
        await parser.Parse(source).WillFail();

        var source2 = "ABC";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('A'));
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

    [Test]
    public async Task LetterTest()
    {
        // Creates a parser that matches any Unicode letter character.

        var parser = Letter();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task LetterOrDigitTest()
    {
        // Creates a parser that matches any Unicode letter or digit character.

        var parser = LetterOrDigit();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source3 = "!@#";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task UpperTest()
    {
        // Creates a parser that matches any Unicode uppercase letter.

        var parser = Upper();

        var source = "ABC";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('A'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task LowerTest()
    {
        // Creates a parser that matches any Unicode lowercase letter.

        var parser = Lower();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "ABC";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task DigitTest()
    {
        // Creates a parser that matches any Unicode digit character.

        var parser = Digit();

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SymbolTest()
    {
        // Creates a parser that matches any Unicode symbol character.

        var parser = Symbol();

        var source = "+=-";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('+'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SeparatorTest()
    {
        // Creates a parser that matches any Unicode separator character.

        var parser = Separator();

        var source = " abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(' '));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task PunctuationTest()
    {
        // Creates a parser that matches any Unicode punctuation character.

        var parser = Punctuation();

        var source = ".,;!?";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('.'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task NumberTest()
    {
        // Creates a parser that matches any Unicode number character.

        var parser = Number();

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();

        var source3 = "Ⅻ"; // Roman numeral twelve
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo('Ⅻ'));
    }

    [Test]
    public async Task SurrogateTest()
    {
        // Creates a parser that matches any surrogate character.

        var parser = Surrogate();

        var source = "\uD800\uDC00";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\uD800'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task HighSurrogateTest()
    {
        // Creates a parser that matches any high surrogate character.

        var parser = HighSurrogate();

        var source = "\uD800\uDC00";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\uD800'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task LowSurrogateTest()
    {
        // Creates a parser that matches any low surrogate character.

        var parser = LowSurrogate();

        var source = "\uDC00\uD800";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\uDC00'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task ControlCharTest()
    {
        // Creates a parser that matches any Unicode control character.

        var parser = ControlChar();

        var source = "\n\r\t";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task WhitespaceTest()
    {
        // Creates a parser that matches any Unicode whitespace character.

        var parser = Whitespace();

        var source = " \t\n";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(' '));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task NewLineTest()
    {
        // Creates a parser that matches a newline (LF) character.

        var parser = NewLine();

        var source = "\nabc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task TabTest()
    {
        // Creates a parser that matches a tab character.

        var parser = Tab();

        var source = "\tabc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('\t'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task AsciiTest()
    {
        // Creates a parser that matches any ASCII character.

        var parser = Ascii();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source3 = "日本語";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task AsciiLetterTest()
    {
        // Creates a parser that matches any ASCII letter.

        var parser = AsciiLetter();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task AsciiUpperLetterTest()
    {
        // Creates a parser that matches any ASCII uppercase letter.

        var parser = AsciiUpperLetter();

        var source = "ABC";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('A'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task AsciiLowerLetterTest()
    {
        // Creates a parser that matches any ASCII lowercase letter.

        var parser = AsciiLowerLetter();

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "ABC";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task AsciiDigitTest()
    {
        // Creates a parser that matches any ASCII digit.

        var parser = AsciiDigit();

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task OctDigitTest()
    {
        // Creates a parser that matches any ASCII octal digit (0-7).

        var parser = OctDigit();

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source2 = "890";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task DecDigitTest()
    {
        // Creates a parser that matches any ASCII decimal digit (0-9).
        // Same as `AsciiDigit`.

        var parser = DecDigit();

        var source = "0123456789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('0'));

        var source2 = "abc";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task HexDigitTest()
    {
        // Creates a parser that matches any ASCII hexadecimal digit (0-9, a-f, A-F).

        var parser = HexDigit();

        var source = "0123456789abcdefABCDEF";
        await Many(parser).End().Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(source));

        var source2 = "xyz";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task HexUpperDigitTest()
    {
        // Creates a parser that matches any ASCII uppercase hexadecimal digit (0-9, A-F).

        var parser = HexUpperDigit();

        var source = "0123456789ABCDEF";
        await Many(parser).End().Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(source));

        var source2 = "abcdef";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task HexLowerDigitTest()
    {
        // Creates a parser that matches any ASCII lowercase hexadecimal digit (0-9, a-f).

        var parser = HexLowerDigit();

        var source = "0123456789abcdef";
        await Many(parser).End().Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(source));

        var source2 = "ABCDEF";
        await parser.Parse(source2).WillFail();
    }
}
