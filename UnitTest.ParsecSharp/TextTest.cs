using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class TextTest
    {
        [TestMethod]
        public void CharTest()
        {
            // Creates a parser that matches a specified character.
            // Similar to the generic `Token` parser but optimized for char, offering better performance.

            var parser = Char('a');

            var source = "abcdEFGH";
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = "123456";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void CharIgnoreCaseTest()
        {
            // Creates a parser that matches a specified character, ignoring case.

            var parser = CharIgnoreCase('A');

            var source = "abcdEFGH";
            parser.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void StringTest()
        {
            // Creates a parser that matches a specified string.

            var parser = String("abc");

            var source = "abcdEFGH";
            parser.Parse(source).WillSucceed(value => value.Is("abc"));
        }

        [TestMethod]
        public void StringIgnoreCaseTest()
        {
            // Creates a parser that matches a specified string, ignoring case.

            var parser = StringIgnoreCase("abcde");

            var source = "abcdEFGH";
            parser.Parse(source).WillSucceed(value => value.Is("abcdE"));
        }

        [TestMethod]
        public void ToIntTest()
        {
            // A combinator that converts the result string to an integer.

            var source = "1234abcd";

            // Parser that matches [0 - 9] and converts to int.
            var parser = Many1(DecDigit()).ToInt();

            parser.Parse(source).WillSucceed(value => value.Is(1234));

            // Fails if the value is non-numeric.
            var parser2 = Many1(Any()).ToInt();
            parser2.Parse(source).WillFail(failure => failure.Message.Is("Expected digits but was '1234abcd'"));

            // Fails if the value exceeds 32-bit range.
            var source2 = "1234567890123456";
            parser.Parse(source2).WillFail(failure => failure.Message.Is("Expected digits but was '1234567890123456'"));
        }

        [TestMethod]
        public void ToLongTest()
        {
            // A combinator that converts the result string to a long integer.

            var source = "1234abcd";

            // Parser that matches [0 - 9] and converts to long.
            var parser = Many1(DecDigit()).ToLong();

            parser.Parse(source).WillSucceed(value => value.Is(1234L));

            // Fails if the value is non-numeric.
            var parser2 = Many1(Any()).ToLong();
            parser2.Parse(source).WillFail(failure => failure.Message.Is("Expected digits but was '1234abcd'"));

            // Can convert values up to 64-bit range.
            var source2 = "1234567890123456";
            parser.Parse(source2).WillSucceed(value => value.Is(1234567890123456L));
        }

        [TestMethod]
        public void ToDoubleTest()
        {
            // A combinator that converts the result string to a double.

            var source = "1234.5678abcd";

            // Parser that matches [0 - 9] / '.' and converts to double.
            var parser = Many1(DecDigit() | Char('.')).ToDouble();

            parser.Parse(source).WillSucceed(value => value.Is(1234.5678));

            // Fails if the value is non-numeric.
            var parser2 = Many1(Any()).ToDouble();
            parser2.Parse(source).WillFail(failure => failure.Message.Is("Expected number but was '1234.5678abcd'"));

            // Supports strings that can be converted using `double.Parse`.
            var source2 = "1.234567890123456";
            parser.Parse(source2).WillSucceed(value => value.Is(1.234567890123456));
        }

        [TestMethod]
        public void OneOfIgnoreCaseTest()
        {
            // Creates a parser that succeeds if the token is in the specified string, ignoring case.

            // Parser that matches [x-z], ignoring case.
            var parser = OneOfIgnoreCase("xyz");

            var source = "ZZZ";
            parser.Parse(source).WillSucceed(value => value.Is('Z'));

            var source2 = "ABC";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void NoneOfIgnoreCaseTest()
        {
            // Creates a parser that succeeds if the token is not in the specified string, ignoring case.

            // Parser that matches anything except [x-z], ignoring case.
            var parser = NoneOfIgnoreCase("xyz");

            var source = "ZZZ";
            parser.Parse(source).WillFail();

            var source2 = "ABC";
            parser.Parse(source2).WillSucceed(value => value.Is('A'));
        }
    }
}
