using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests.Parser;

public class StreamControlPrimitivesTests
{
    [Test]
    public async Task EndOfInputTest()
    {
        // Creates a parser that matches the end of the input.

        var parser = EndOfInput();

        var source = string.Empty;
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task NullTest()
    {
        // Creates a parser that matches an empty string and always succeeds in any state.
        // This parser does not consume input.

        var parser = Null();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = string.Empty;
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task ConditionTest()
    {
        // Creates a parser that succeeds or fails based on a boolean condition without consuming input.
        // Useful for conditional parsing logic and validation.

        // Parser that succeeds when condition is true.
        var parser = Condition(true);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        // Parser that fails when condition is false.
        var parser2 = Condition(false);

        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Given condition was false"));

        // Can provide custom error message.
        var parser3 = Condition(false, "Custom validation failed");

        await parser3.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Custom validation failed"));
    }
}
