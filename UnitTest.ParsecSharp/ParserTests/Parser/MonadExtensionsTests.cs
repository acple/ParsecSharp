using System;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class MonadExtensionsTests
{
    [Test]
    public async Task BindTest()
    {
        // Monad bind operation.
        // Sequentially composes two parsers, passing the result of the first to a function that returns the second parser.

        // Parser that matches a digit to determine how many letters to parse next.
        var parser = Many1(AsciiDigit()).ToInt().Bind(count => AsciiLetter().Repeat(count).AsString());

        var source = "1abcdef";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("a"));

        var source2 = "3abcdef";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

        var source3 = "7abcdef";
        await parser.Parse(source3).WillFail();

        var source4 = "123456";
        await parser.Parse(source4).WillFail();
    }

    [Test]
    public async Task AlternativeTest()
    {
        // Combines two parsers, attempting the second parser if the first fails.
        // This is the fundamental choice operation for parsing alternatives.

        // Parser that matches either a letter or a digit.
        var parser = AsciiLetter().Alternative(DecDigit());

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source3 = "!@#";
        await parser.Parse(source3).WillFail();

        // Provides an alternative operator using the `|` syntax.
        var parser2 = DecDigit() | Char('!');

        var source4 = "!abc";
        await parser2.Parse(source4).WillSucceed(async value => await Assert.That(value).IsEqualTo('!'));
    }

    [Test]
    public async Task MapTest()
    {
        // Functor map operation. Transforms the result of a parser using a function.

        // Parser that matches three lowercase letters and converts the resulting string to uppercase.
        var parser = AsciiLowerLetter().Repeat(3).AsString().Map(x => x.ToUpperInvariant());

        var source = "abcdef";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ABC"));

        // Parser that matches a letter and converts it to uppercase.
        var parser2 = AsciiUpperLetter().Map(char.ToLowerInvariant);

        var source2 = "ABC";
        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source3 = "abc";
        await parser2.Parse(source3).WillFail();
    }

    [Test]
    public async Task MapConstTest()
    {
        // Maps the parser result to a constant value, discarding the original parsed value.
        // Useful when you only care about success/failure, not the actual parsed value.

        // Parser that matches any letter and always returns the string "42".
        var parser = Letter().MapConst("42");

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("42"));

        var source2 = "xyz";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("42"));

        var source3 = "123";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task MapWithExceptionHandlingTest()
    {
        // Maps the parser result with automatic exception handling, converting exceptions to parse failures.
        // Unlike Map, exceptions thrown in the mapping function are caught and converted to Failure results.

        // Parser that attempts to parse a string and convert it to uint, catching exceptions.
        var parser = Any().Repeat(3).AsString().MapWithExceptionHandling(uint.Parse);

        var source = "123abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(123u));

        var source2 = "abcdef";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.Exception.InnerException).IsTypeOf<FormatException>());

        // Parser that attempts division by zero, catching the exception.
        var parser2 = DecDigit().AsString().ToInt().MapWithExceptionHandling(n => 10 / n);

        var source3 = "5rest";
        await parser2.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo(2));

        var source4 = "0rest";
        await parser2.Parse(source4).WillFail(async failure => await Assert.That(failure.Exception.InnerException).IsTypeOf<DivideByZeroException>());
    }

    [Test]
    public async Task EitherTest()
    {
        // Transforms both success and failure cases of a parser.
        // This always succeeds by converting failures into successful results.

        // Parser that attempts to parse a digit, returning the digit on success or -1 on failure.
        var parser = DecDigit().AsString().ToInt().Either(x => x * 10, -1);

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(10));

        var source2 = "abc";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(-1));

        // Parser that uses a function to compute the fallback value based on the failure.
        var parser2 = Many1(AsciiLetter()).AsString().Either(x => x.ToUpperInvariant(), failure => $"Error: {failure.Message}");

        var source3 = "abc";
        await parser2.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("ABC"));

        var source4 = "123";
        await parser2.Parse(source4).WillSucceed(async value => await Assert.That(value).Contains("Error"));
    }

    [Test]
    public async Task NextTest()
    {
        // A combinator that allows direct description of continuation processing for parser.

        // Parser that matches `Letter`, returns the result of matching the next `Letter` if successful, and returns '\n' if it fails.
        var parser = Letter().Next(_ => Letter(), '\n');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('b'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        // The overall result is failure because the first `Letter` succeeded but the next `Letter` failed.
        var source3 = "a123";
        await parser.Parse(source3).WillFail();

        // The overload that takes a parser for failure case.
        var parser2 = Char('a').Next(_ => Char('b'), _ => Digit());

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('b'));

        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        await parser2.Parse(source3).WillFail();
    }

    [Test]
    public async Task FlattenTest()
    {
        // Flattens a nested parser structure. Converts IParser<TToken, IParser<TToken, T>> to IParser<TToken, T>.
        // This is equivalent to join/flatten in monadic operations.

        // Parser that matches a digit to determine how many characters to parse next.
        var nestedParser = DecDigit().AsString().ToInt().Map(Any().Repeat);

        var parser = nestedParser.Flatten();

        var source = "3abcdef";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abc"));

        var source2 = "5abcdef";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcde"));

        var source3 = "abcdef";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task GuardTest()
    {
        // Branches success and failure based on a condition for the result matched by parser.

        // Parser that matches a number and succeeds only if it is less than 1000.
        var parser = Many1(DecDigit()).ToInt().Guard(x => x < 1000);

        var source = "123456";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("A value '123456' does not satisfy condition"));

        var source2 = "999";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(999));

        // If the parser does not match, the validation itself is not performed.
        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillFail();
    }
}
