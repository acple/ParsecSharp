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
        {
            // Creates a parser that matches parser count times and returns the result as a sequence.

            // 2*( 3*Any )
            var parser = Any().Repeat(3).AsString().Repeat(2);

            var source = "abcdEFGH";
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["abc", "dEF"]));
        }

        {
            // Creates a parser that matches parser between min and max times (inclusive).

            // Matches 2 to 4 digits.
            var parser = Digit().Repeat(2, 4).AsString();

            // Succeeds when the number of matches is within [min, max].
            await parser.Parse("1234abcd").WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));

            // Succeeds with fewer matches when subsequent tokens don't match, as long as min is satisfied.
            await parser.Parse("12abcd").WillSucceed(async value => await Assert.That(value).IsEqualTo("12"));

            // Succeeds with the maximum count even if more matching tokens follow.
            await parser.Parse("123456").WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));

            // Fails when the number of matches is less than min.
            await parser.Parse("1abcd").WillFail();

            // Repeat consumes only up to max tokens and leaves remaining input for subsequent parsers.
            var parser2 = Digit().Repeat(2, 4).AsString().Repeat(2);

            await parser2.Parse("123456789").WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["1234", "5678"]));
            await parser2.Parse("1234567ab").WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["1234", "567"]));
            await parser2.Parse("123456abc").WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(["1234", "56"]));
            await parser2.Parse("12345abcd").WillFail(); // Fails when total input is insufficient for two repetitions.

            // To ensure the input has no more matches beyond max, combine with Not.
            var parser3 = Digit().Repeat(2, 4).Left(Not(Digit())).AsString();

            await parser3.Parse("1234abcd").WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));
            await parser3.Parse("123456").WillFail();
        }
    }

    [Test]
    public async Task QuotedByTest()
    {
        // Creates a parser that matches parser repeatedly until it matches the parsers before and after.

        // Parser that matches a string representation that can escape '"' characters.
        var dquoteOrAny = String("\\\"").Map(_ => '\"') | Any();
        var parser = dquoteOrAny.QuotedBy(Char('"')).AsString();

        var source = """
            "abcd\"EFGH"
            """;
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("""abcd"EFGH"""));

        // Succeeds with empty content between '"' and '"'.
        var source2 = "\"\"";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());

        // Parser that matches letters between '[' and ']'.
        var parser2 = Letter().QuotedBy(Char('['), Char(']')).AsString();

        var source3 = "[abcd]";
        await parser2.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));

        // Succeeds with empty content between '[' and ']'.
        var source4 = "[]";
        await parser2.Parse(source4).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task QuotedBy1Test()
    {
        // Creates a parser that matches parser repeatedly until it matches the parsers before and after.
        // Similar to `QuotedBy`, but requires at least 1 match of the parser.

        // Parser that matches a string representation that can escape '"' characters.
        var dquoteOrAny = String("\\\"").Map(_ => '\"') | Any();
        var parser = dquoteOrAny.QuotedBy1(Char('"')).AsString();

        var source = """
            "abcd\"EFGH"
            """;
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("""abcd"EFGH"""));

        // Fails because there is no content between '"' and '"'.
        var source2 = "\"\"";
        await parser.Parse(source2).WillFail();

        // Parser that matches letters between '[' and ']'.
        var parser2 = Letter().QuotedBy1(Char('['), Char(']')).AsString();

        var source3 = "[abcd]";
        await parser2.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));

        // Fails because there is no letter between '[' and ']'.
        var source4 = "[]";
        await parser2.Parse(source4).WillFail();
    }
}
