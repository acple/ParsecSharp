using System;
using System.Linq;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class IterativeApplicationExtensionsTests
{
    [Test]
    public async Task ChainTest()
    {
        // Creates a recursive parser that starts with a single parser, creates the next parser based on the result, and repeats until it fails.

        // Parser that matches any character consecutively, and returns the matched character and its count as a result.
        var parser = Any().Map(x => (x, count: 1))
            .Chain(match => Char(match.x).Map(_ => (match.x, match.count + 1)))
            .Map(match => match.x.ToString() + match.count.ToString());

        var source = "aaaaaaaaa";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("a9"));

        var source2 = "aaabbbbcccccdddddd";
        await Many(parser).Join().Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("a3b4c5d6"));

        // Originally, it is not possible to directly describe a parser that references itself first
        // (because it would result in infinite recursion).

        // A famous left-recursive definition of binary operations.
        // expr = expr op digit / digit
        static IParser<char, int> Expr()
            => (from x in Expr() // Infinite recursion here
                from func in Char('+')
                from y in Num()
                select x + y)
                .Or(Num());

        static IParser<char, int> Num()
            => Many1(Digit()).ToInt();

        // It is possible to transform this definition to remove left recursion.
        // Definition of binary operations after removing left recursion.
        // expr = digit *( op digit )
        static IParser<char, int> Expr2()
            => Num().Chain(x => Char('+').Right(Num()).Map(y => x + y));
        // By using `Chain`, you can directly describe the definition after removing left recursion.

        await Expr2().Parse("1+2+3+4").WillSucceed(async value => await Assert.That(value).IsEqualTo(1 + 2 + 3 + 4));
    }

    [Test]
    public async Task ChainLeftTest()
    {
        // Creates a parser that matches 1 or more values and operators alternately, and applies the specified operation from left to right.

        // Parser that matches '+' or '-', and returns a binary operation function (x + y) or (x - y).
        // ( "+" / "-" )
        var op = Choice(
            Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
            Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

        // Parser that matches 1 or more digits and converts them to int.
        var num = Many1(Digit()).ToInt();

        // num *( op num )
        var parser = num.ChainLeft(op);

        var source = "10+5-3+1";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(((10 + 5) - 3) + 1));

        var source2 = "100-20-5+50";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(((100 - 20) - 5) + 50));

        var source3 = "123";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo(123));

        var source4 = "abcdEFGH";
        await parser.Parse(source4).WillFail();

        var source5 = "1-2+3+ABCD";
        await parser.Parse(source5).WillSucceed(async value => await Assert.That(value).IsEqualTo((1 - 2) + 3));
        await parser.Right(Any()).Parse(source5).WillSucceed(async value => await Assert.That(value).IsEqualTo('+'));
    }

    [Test]
    public async Task ChainRightTest()
    {
        // Creates a parser that matches 1 or more values and operators alternately, and applies the specified operation from right to left.

        // Parser that matches '+' or '-', and returns a binary operation function (x + y) or (x - y).
        // ( "+" / "-" )
        var op = Choice(
            Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
            Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

        // Parser that matches 1 or more digits and converts them to int.
        var num = Many1(Digit()).ToInt();

        // num *( op num )
        var parser = num.ChainRight(op);

        var source = "10+5-3+1";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(10 + (5 - (3 + 1))));

        var source2 = "100-20-5+50";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(100 - (20 - (5 + 50))));

        var source3 = "123";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo(123));

        var source4 = "abcdEFGH";
        await parser.Parse(source4).WillFail();

        var source5 = "1-2+3+ABCD";
        await parser.Parse(source5).WillSucceed(async value => await Assert.That(value).IsEqualTo(1 - (2 + 3)));
        await parser.Right(Any()).Parse(source5).WillSucceed(async value => await Assert.That(value).IsEqualTo('+'));
    }

    [Test]
    public async Task FoldLeftTest()
    {
        // Takes an initial value and an aggregation function as arguments, and creates a parser that aggregates the parsed results from left to right.

        // Parser that matches 0 or more digits, and repeatedly applies (x => accumulator - x) to the initial value 10 from the left.
        var parser = Digit().AsString().ToInt().FoldLeft(10, (x, y) => x - y);

        var source = "12345";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(((((10 - 1) - 2) - 3) - 4) - 5));

        var source2 = "abcdEFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(10)); // No match, returns initial value

        // Overload that does not use an initial value.
        var parser2 = Digit().AsString().ToInt().FoldLeft((x, y) => x - y);
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo((((1 - 2) - 3) - 4) - 5));

        await parser2.Parse(source2).WillFail();
    }

    [Test]
    public async Task FoldRightTest()
    {
        // Takes an initial value and an aggregation function as arguments, and creates a parser that aggregates the parsed results from right to left.

        // Parser that matches 0 or more digits, and repeatedly applies (x => x - accumulator) to the initial value 10 from the right.
        var parser = Digit().AsString().ToInt().FoldRight(10, (x, y) => x - y);

        var source = "12345";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1 - (2 - (3 - (4 - (5 - 10))))));

        var source2 = "abcdEFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(10)); // No match, returns initial value

        // Overload that does not use an initial value.
        var parser2 = Digit().AsString().ToInt().FoldRight((x, y) => x - y);
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1 - (2 - (3 - (4 - 5)))));

        await parser2.Parse(source2).WillFail();
    }
}
