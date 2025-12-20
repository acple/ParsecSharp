using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserSequencingCombinatorsTests
{
    [Test]
    public async Task SequenceTest()
    {
        // Creates a parser that matches parsers in sequence and returns the concatenated result as a sequence.

        // Parser that matches 'a' + 'b' + 'c' + 'd' and converts to "abcd".
        var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).AsString();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));

        var source2 = "abCDEF";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 3): Unexpected 'C<0x43>'"));

        // You can pass an arbitrary number of parsers using the `params IEnumerable<T>` overload.
        var parser2 = Sequence(Char('a'), Char('b'), Char('c'), Char('d'), Char('E'), Char('F'), Char('G'), Char('H'), Pure('_')).AsString();

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH_"));

        await parser2.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 3): Unexpected 'C<0x43>'"));

        // `Sequence` has overloads to handle parsers with different types, supporting up to 8 parsers.
        var parser3 = Sequence(Char('a'), String("bc"), HexDigit(), SkipMany(Upper()), Pure(999), (a, bc, d, _, i) => new { a, bc, d, i });

        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(new { a = 'a', bc = "bc", d = 'd', i = 999 }));
    }
}
