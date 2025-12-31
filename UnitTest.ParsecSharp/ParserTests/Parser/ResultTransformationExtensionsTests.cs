using System.Linq;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class ResultTransformationExtensionsTests
{
    [Test]
    public async Task IgnoreTest()
    {
        // Discards the parsed result.
        // Used when you want to match the type or explicitly discard the value.

        // Parser that matches 1 or more lowercase letters and discards the result.
        var parser = Many1(Lower()).Ignore();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        await parser.Right(Any()).Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('E'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task MapToStringTest()
    {
        // Converts the parsed result to a string representation.

        var parser = Many1(Digit()).AsString().ToInt().MapToString();

        var source = "12345";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("12345"));

        // Handles null values by returning empty string.
        var parser2 = Pure<char, object?>(null as object).MapToString();
        await parser2.Parse(string.Empty).WillSucceed(async value => await Assert.That(value).IsEqualTo(string.Empty));
    }

    [Test]
    public async Task ToArrayTest()
    {
        // Converts an IEnumerable<T> result to an array.

        var parser = Many1(Letter()).ToArray();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsTypeOf<char[]>().And.IsSequentiallyEqualTo("abcdEFGH"));
    }

    [Test]
    public async Task AsEnumerableTest()
    {
        // Identity function that returns the parser as-is when the result is IEnumerable<T>.
        // Useful for type conversion or making intent explicit.

        var parser = Many1(Letter()).AsEnumerable();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
    }

    [Test]
    public async Task AsReadOnlyCollectionTest()
    {
        // Identity function that returns the parser as-is when the result is IReadOnlyCollection<T>.

        var parser = Many1(Letter()).AsReadOnlyCollection();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
    }

    [Test]
    public async Task AsReadOnlyListTest()
    {
        // Identity function that returns the parser as-is when the result is IReadOnlyList<T>.

        var parser = Many1(Letter()).ToArray().AsReadOnlyList();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
    }

    [Test]
    public async Task FlattenTest()
    {
        // A combinator that flattens a parser that results in a nested `IEnumerable<T>`.

        var source = "abcdEFGH";

        // Parser that takes 2 tokens.
        var token = Any().Repeat(2);
        // Parser that matches the parser that takes 2 tokens 1 or more times.
        var parser = Many1(token);
        // Parser that flattens the result of parser.
        var parser2 = parser.Flatten();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).Count().IsEqualTo(4).And.All(x => x.Count == 2));

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));

        // In situations where it becomes nested due to using `Many1`, you can use `FoldLeft` instead.
        var parser3 = token.AsEnumerable().FoldLeft((x, y) => x.Concat(y));
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo("abcdEFGH"));
    }

    [Test]
    public async Task SingletonTest()
    {
        // A combinator that returns the result matched by parser as a single-element sequence.

        // Parser that matches 'a' and returns [ 'a' ].
        var parser = Char('a').Singleton();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value.Count).IsEqualTo(1));
    }
}
