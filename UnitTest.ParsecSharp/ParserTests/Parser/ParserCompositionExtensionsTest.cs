using System.Linq;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ParserCompositionExtensionsTest
{
    [Test]
    public async Task LeftTest()
    {
        // Creates a parser that matches two parsers in sequence and returns the result of left, discarding the result of right.

        // Parser that matches ( "a" "b" ) and returns 'a'.
        var parser = Char('a').Left(Char('b'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "a";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task RightTest()
    {
        // Creates a parser that matches two parsers in sequence and returns the result of right, discarding the result of left.

        // Parser that matches ( "a" "b" ) and returns 'b'.
        var parser = Char('a').Right(Char('b'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('b'));

        var source2 = "b";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task BetweenTest()
    {
        // Creates a parser that matches parser enclosed by open and close.
        // The results of open and close are discarded, and only the result of the middle parser is returned.

        // Parser that matches 1 or more letters enclosed in "[]".
        // ( "[" 1*Letter "]" )
        var parser = Many1(Letter()).Between(Char('['), Char(']'));

        var source = $"[abcdEFGH]";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));

        // If you pass `Many(Any())` to the parser, it will match any input until the end, so `close` will match the end of input.
        var parser2 = Many(Any()).Between(Char('"'), Char('"')); // It does not match ( dquote *Any dquote )
        await parser2.Parse("\"abCD1234\"").WillFail(); // `Many(Any())` matches until abCD1234", so `close` does not match " and fails
        // If you want to create a parser that matches this form, consider using `Quote` or `ManyTill`.
    }

    [Test]
    public async Task OrTest()
    {
        // Alias for `Alternative`. Combines two parsers, attempting the second parser if the first fails.
        // Provides a more readable name for choice operations.

        // Parser that matches either a letter or a digit.
        var parser = Letter().Or(Digit());

        var source = "abc";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source3 = "!@#";
        await parser.Parse(source3).WillFail();

        // You can use operator for more readability.
        var parser2 = DecDigit() | Char('!');

        var source4 = "!abc";
        await parser2.Parse(source4).WillSucceed(async value => await Assert.That(value).IsEqualTo('!'));
    }

    [Test]
    public async Task ExceptTest()
    {
        // Creates a parser with exclusion conditions for the specified parser.

        // Parser that matches digits except '5'.
        var parser = Digit().Except(Char('5'));

        var source = "123";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        var source2 = "567";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Unexpected succeed with value '5'"));

        // Parser that matches digits except '5' consecutively and returns the result as a string.
        var parser2 = Many(parser).AsString();

        var source3 = "123456";
        await parser2.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));
    }

    [Test]
    public async Task AppendTest()
    {
        // Matches two parsers in sequence and returns the concatenated result as a sequence.

        var source = "abcdEFGH";

        // IReadOnlyList<T>
        {
            // 1 + 1
            {
                var parser = Any<int>().Append(Any<int>());
                await parser.Parse([1, 2]).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2]));
                await parser.Parse([1]).WillFail();
            }
        }

        // IReadOnlyCollection<T>
        {
            // n + 1
            {
                var parser = Many1(Lower()).Append(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }

            // 1 + n
            {
                var parser = Any().Append(Many1(Lower()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillFail();
            }

            // n + n
            {
                var parser = Many1(Lower()).Append(Many1(Upper()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }
        }

        // IEnumerable<T>
        {
            // n + 1
            {
                var parser = Many1(Lower()).AsEnumerable().Append(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }

            // 1 + n
            {
                var parser = Any().Append(Many1(Lower()).AsEnumerable());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillFail();
            }

            // n + n
            {
                var parser = Many1(Lower()).AsEnumerable().Append(Many1(Upper()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }
        }

        // string
        {
            // 1 + 1
            {
                var parser = Any().Append(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));
                var source2 = "a";
                await parser.Parse(source2).WillFail();
            }

            // n + 1
            {
                var parser = Many1(Lower()).AsString().Append(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }

            // 1 + n
            {
                var parser = Any().Append(Many1(Lower()).AsString());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillFail();
            }

            // n + n
            {
                var parser = Many1(Lower()).AsString().Append(Many1(Upper()).AsString());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillFail();
            }
        }
    }

    [Test]
    public async Task AppendOperatorTest()
    {
        // `Append` can also be described using operators.

        // generic collections
        {
            var a = Pure(1);
            var b = Pure(2);
            var c = Pure(3);
            var d = Pure(4);

            var source = string.Empty;

            // IReadOnlyList<T>
            {
                var appendTwo = a + b;
                await appendTwo.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2]));
            }

            // IReadOnlyCollection<T>
            {
                var leftAssociative = a + b + c;
                await leftAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3]));

                var rightAssociative = a + (b + c);
                await rightAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3]));

                var appendCollection = (a + b) + (c + d);
                await appendCollection.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3, 4]));
            }

            // IEnumerable<T>
            {
                var leftAssociative = (a + b).AsEnumerable() + c;
                await leftAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3]));

                var rightAssociative = a + (b + c).AsEnumerable();
                await rightAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3]));

                var appendCollection = (a + b).AsEnumerable() + (c + d).AsEnumerable();
                await appendCollection.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2, 3, 4]));
            }
        }

        // string
        {
            var a = Char('a');
            var b = Char('b');
            var c = Char('c');
            var d = Char('d');

            var source = "abcdEFGH";

            var appendTwo = a + b;
            await appendTwo.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));

            var leftAssociative = a + b + c;
            await leftAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

            var rightAssociative = a + (b + c);
            await rightAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

            var appendString = (a + b) + (c + d);
            await appendString.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }
    }

    [Test]
    public async Task AppendOptionalTest()
    {
        // Behaves like `Append`, but if the right parser fails, it treats the result as an empty sequence and combines them.

        var source = "abcdEFGH";

        // IReadOnlyList<T>
        {
            // 1 + 1
            {
                var parser = Any<int>().AppendOptional(Any<int>());
                await parser.Parse([1, 2]).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1, 2]));
                await parser.Parse([1]).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo([1]));
            }
        }

        // IReadOnlyCollection<T>
        {
            // n + 1
            {
                var parser = Many1(Lower()).AppendOptional(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
            }

            // 1 + n
            {
                var parser = Any().AppendOptional(Many1(Lower()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("A"));
            }

            // n + n
            {
                var parser = Many1(Lower()).AppendOptional(Many1(Upper()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
            }
        }

        // IEnumerable<T>
        {
            // n + 1
            {
                var parser = Many1(Lower()).AsEnumerable().AppendOptional(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
            }

            // 1 + n
            {
                var parser = Any().AppendOptional(Many1(Lower()).AsEnumerable());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("A"));
            }

            // n + n
            {
                var parser = Many1(Lower()).AsEnumerable().AppendOptional(Many1(Upper()));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcd"));
            }
        }

        // string
        {
            // 1 + 1
            {
                var parser = Any().AppendOptional(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));
                var source2 = "a";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("a"));
            }

            // n + 1
            {
                var parser = Many1(Lower()).AsString().AppendOptional(Any());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdE"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
            }

            // 1 + n
            {
                var parser = Any().AppendOptional(Many1(Lower()).AsString());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
                var source2 = "ABCD";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("A"));
            }

            // n + n
            {
                var parser = Many1(Lower()).AsString().AppendOptional(Many1(Upper()).AsString());
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
                var source2 = "abcd";
                await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
            }
        }
    }
}
