using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class RepetitionControlCombinatorsTests
{
    [Test]
    public async Task ManyTest()
    {
        // Creates a parser that matches parser 0 or more times and returns the result as a sequence.
        // If it does not match even once, the parser returns an empty sequence. In this case, it does not consume input.

        // Parser that matches `Lower` 0 or more times.
        var parser = Many(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task Many1Test()
    {
        // Creates a parser that matches parser 1 or more times and returns the result as a sequence.
        // If it does not match even once, the parser returns a failure.

        // Parser that matches `Lower` 1 or more times.
        var parser = Many1(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SkipManyTest()
    {
        // Creates a parser that matches parser 0 or more times and discards the result.
        // If it does not match even once, it does not consume input.

        // Parser that matches `Lower` 0 or more times, discards the result, then matches `Any`.
        var parser = SkipMany(Lower()).Right(Any());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('E'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));
    }

    [Test]
    public async Task SkipMany1Test()
    {
        // Creates a parser that matches parser 1 or more times and discards the result.

        // Parser that matches `Lower` 1 or more times, discards the result, then matches `Any`.
        var parser = SkipMany1(Lower()).Right(Any());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('E'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task ManyTillTest()
    {
        // Creates a parser that matches parser 0 or more times until terminator is matched and returns the result as a sequence.
        // The result of matching terminator is discarded.

        // Parser that matches `Lower` 0 or more times until 'E' is matched.
        var parser = ManyTill(Lower(), Char('E'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());

        // If terminator is not matched, the parser fails.
        var source3 = "abcd1234";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task Many1TillTest()
    {
        // Creates a parser that matches parser 1 or more times until terminator is matched and returns the result as a sequence.
        // The result of matching terminator is discarded.

        // Parser that matches `Lower` 1 or more times until 'E' is matched.
        var parser = Many1Till(Lower(), Char('E'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillFail();

        var source3 = "abcd1234";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task SkipTillTest()
    {
        // Creates a parser that skips parser 0 or more times until it matches terminator and returns the result of matching terminator.

        // Parser that skips `Lower` 0 or more times until it matches "EF".
        var parser = SkipTill(Lower(), String("EF"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("EF"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("EF"));

        // Fails because an uppercase letter exists before "EF".
        var source3 = "abCDEFGH";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task Skip1TillTest()
    {
        // Creates a parser that skips parser 1 or more times until it matches terminator and returns the result of matching terminator.

        // Parser that skips `Lower` 1 or more times until it matches "EF".
        var parser = Skip1Till(Lower(), String("EF"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("EF"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillFail();

        var source3 = "abCDEFGH";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task TakeTillTest()
    {
        // Creates a parser that reads input 0 or more times until it matches terminator and returns the result as a sequence.
        // The result of matching terminator is discarded.

        // Parser that reads input 0 or more times until it matches 'E'.
        var parser = TakeTill(Char('E'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());

        // If terminator is not matched, the parser fails.
        var source3 = "123456";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task Take1TillTest()
    {
        // Creates a parser that reads input 1 or more times until it matches terminator and returns the result as a sequence.
        // The result of matching terminator is discarded.

        // Parser that reads input 1 or more times until it matches 'E'.
        var parser = Take1Till(Char('E'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));

        var source2 = "EFGH";
        await parser.Parse(source2).WillFail();

        var source3 = "123456";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task MatchTest()
    {
        // Creates a parser that skips until it matches parser and returns the result of matching parser.

        // Parser that skips until it matches "FG".
        var parser = Match(String("FG"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("FG"));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();

        // Parser that skips until it matches `Lower` + `Upper`.
        var parser2 = Match(Lower() + Upper());

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("dE"));
    }

    [Test]
    public async Task QuotedTest()
    {
        // Creates a parser that retrieves the sequence of tokens between the parsers that match before and after.
        // Can be used to retrieve tokens like string literals.
        // Use the `Quote` extension method if you want to add conditions to the match of the token sequence.

        // Parser that retrieves the string between '<' and '>'.
        var parser = Quoted(Char('<'), Char('>')).AsString();

        var source = "<abcd>";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));

        // Parser that retrieves the string between '<' and '>', and then retrieves the string between them.
        var parser2 = Quoted(parser).AsString();

        var source2 = "<span>test</span>";
        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("test"));
    }
}
