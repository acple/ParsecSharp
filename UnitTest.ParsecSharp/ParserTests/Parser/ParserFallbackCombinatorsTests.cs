using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserFallbackCombinatorsTests
{
    [Test]
    public async Task TryTest()
    {
        // Creates a parser that executes the parse with parser and returns the value of fallback if it fails, always succeeding.
        // If parser fails to match, it does not consume input.

        // Parser that matches 'a' and returns 'a' if successful, 'x' if it fails.
        var parser = Try(Char('a'), 'x');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('x'));

        // Overload that delays the evaluation of fallback.
        var parser2 = Try(Char('a'), _ => 'x');
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task OptionalTest()
    {
        // Creates a parser that executes the parse with parser and returns a bool indicating whether it matched, always succeeding.
        // If the parser fails to match, it does not consume input.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that matches `Digit`, returns boolean value that matches or not, then matches `Any`.
        var parser = Sequence(Optional(Digit()), Any(), (match, token) => (match, token));

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo((match: false, token: 'a')));

        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo((match: true, token: '2')));

        // Overload that returns a specified default value if it fails.
        var parser2 = Optional(Lower(), '\n');

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));
    }
}
