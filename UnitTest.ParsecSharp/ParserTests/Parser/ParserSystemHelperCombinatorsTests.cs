using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserSystemHelperCombinatorsTests
{
    [Test]
    public async Task AtomTest()
    {
        // Treats the specified parser as the smallest unit of the parser.
        // Even if the parsing process fails halfway, it backtracks to the starting point.
        // Designed for use in combination with `WithConsume` / `AbortIfEntered`.

        var abCD = Sequence(Char('a'), Char('b'), Char('C'), Char('D'));
        var parser = Atom(abCD);

        var source = "abcdEFGH";
        await abCD.Parse(source).WillFail(async failure => await Assert.That(failure.State.Position).IsEquivalentTo(new { Line = 1, Column = 3 }));
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.State.Position).IsEquivalentTo(new { Line = 1, Column = 1 }));
    }

    [Test]
    public async Task DelayTest()
    {
        // Delays the construction of the specified parser until the parsing execution.
        // Can also be used as a reference holder, essential for forward references and recursion.

        // Parser that matches 'a'. However, it is constructed at the time of parsing execution.
        var parser = Delay(() => Char('a'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        // The `Func<T>` passed to `Delay` is executed only once at the time of parsing execution.
        // Therefore, be careful as it may behave unexpectedly if the process contains side effects.
        // Reusing this parser object can improve performance.
    }

    [Test]
    public async Task FixTest()
    {
        // A helper combinator for constructing self-recursive parsers in local variables or parser definition expressions.
        // Due to the specifications of C#, it is necessary to provide type arguments when used alone due to lack of type information.

        // Parser that matches a character enclosed in any number of "{}".
        var parser = Fix<char>(self => self.Or(Any()).Between(Char('{'), Char('}')));

        var source = "{{{{{*}}}}}";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('*'));

        // Overload that takes parameters. Allows flexible description of recursive parsers.
        // Famous palindrome parser. S ::= "a" S "a" | "b" S "b" | ""
        var parser2 = Fix<IParser<char, Unit>, Unit>((self, rest) =>
            Char('a').Right(self(Char('a').Right(rest))) | Char('b').Right(self(Char('b').Right(rest))) | rest);

        var source2 = "abbaabba";
        await parser2(EndOfInput()).Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }
}
