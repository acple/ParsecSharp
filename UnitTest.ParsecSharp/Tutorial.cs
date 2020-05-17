using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using ParsecSharp.Examples;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class Tutorial
    {
        [TestMethod]
        public void Usage()
        {
            var source = "aabbb";

            // will succeed
            {
                // define parser
                var parser = String("aa");

                // run parser
                var result = parser.Parse(source);

                {
                    // gets result value or string.Empty
                    var value = result.CaseOf(
                        failure => string.Empty, // failure path
                        success => success.Value); // success path
                    value.Is("aa");
                }

                {
                    // gets result value directly, throws an exception when parsing is failed
                    var value = result.Value;
                    value.Is("aa");
                }
            }

            // will fail
            {
                var parser = String("bb");

                var result = parser.Parse(source);

                {
                    var value = result.CaseOf(
                        failure => string.Empty,
                        success => success.Value);
                    value.Is(string.Empty);
                }

                {
                    ExceptionAssert.Throws<ParsecException>(() =>
                    {
                        _ = result.Value;
                        Assert.Fail($"does not reach here");
                    });
                }
            }
        }

        [TestMethod]
        public void RegexMatchingExample()
        {
            var source = "aabbb";

            {
                {
                    var regex = new Regex("^a*");
                    regex.Match(source).Value.Is("aa");

                    var parser = Many(Char('a'));
                    parser.Parse(source).WillSucceed(value => value.Is('a', 'a'));
                }

                {
                    var regex = new Regex("^b*");
                    regex.Match(source).Value.Is("");

                    var parser = Many(Char('b'));
                    parser.Parse(source).WillSucceed(value => value.Is(Enumerable.Empty<char>()));
                }
            }

            {
                {
                    var regex = new Regex("^a+");
                    regex.Match(source).Value.Is("aa");

                    var parser = Many1(Char('a'));
                    parser.Parse(source).WillSucceed(value => value.Is('a', 'a'));
                }

                {
                    var regex = new Regex("^b+");
                    regex.Match(source).Success.IsFalse();

                    var parser = Many1(Char('b'));
                    parser.Parse(source).WillFail(failure => failure.Message.Is("Unexpected 'a<0x61>'"));
                }
            }

            {
                var regex = new Regex("^a*$");
                regex.Match(source).Success.IsFalse();

                var parser = Many(Char('a')).End();
                parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 3): Expected '<EndOfStream>' but was 'b<0x62>'"));
            }

            {
                {
                    var regex = new Regex("^ab");
                    regex.Match(source).Success.IsFalse();

                    var parser = Sequence(Char('a'), Char('b'));
                    parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 2): Unexpected 'a<0x61>'"));

                    var parser2 = String("ab");
                    parser2.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Expected 'ab' but was 'aa'"));
                }

                {
                    var regex = new Regex("ab");
                    regex.Match(source).Value.Is("ab");

                    var parser = Match(Sequence(Char('a'), Char('b')));
                    parser.Parse(source).WillSucceed(value => value.Is('a', 'b'));

                    var parser2 = Match(String("ab"));
                    parser2.Parse(source).WillSucceed(value => value.Is("ab"));
                }
            }

            {
                var regex = new Regex("^a+b+$");
                regex.Match(source).Value.Is("aabbb");

                var parser = Many1(Char('a')).Append(Many1(Char('b'))).End();
                parser.Parse(source).WillSucceed(value => value.Is('a', 'a', 'b', 'b', 'b'));

                var parser2 = Many1(Char('a')).Append(Many1(Char('b'))).AsString().End();
                parser2.Parse(source).WillSucceed(value => value.Is("aabbb"));
            }

            {
                var regex = new Regex("^a.b");
                regex.Match(source).Value.Is("aab");

                var parser = Sequence(Char('a'), Any(), Char('b'));
                parser.Parse(source).WillSucceed(value => value.Is('a', 'a', 'b'));
            }
        }

        [TestMethod]
        public void JsonParserExample()
        {
            var source = @"{""key1"":123,""key2"":""abc"",""key3"":{""key3_1"":true,""key3_2"":[1,2,3]}}";
            var result = JsonParser.Parse(source)
                .CaseOf(failure => default, success => success.Value);

            var key1 = (double)result?["key1"];
            key1.Is(123.0);

            var key2 = result?["key2"] as string;
            key2.Is("abc");

            var key3_1 = (bool)result?["key3"]?["key3_1"];
            key3_1.IsTrue();

            var key3_2 = result?["key3"]?["key3_2"] as IEnumerable<dynamic>;
            key3_2.Is(1.0, 2.0, 3.0);
        }

        [TestMethod]
        public void CsvParserExample()
        {
            var source = $"{"123,abc,def"}\n{"456,\"escaped\"\"\n\",xyz"}\n{"999,2columns"}\n";
            var result = CsvParser.Parse(source).Value.ToArray();

            result.Length.Is(3);

            result[0].Is(record => record[0] == "123" && record[1] == "abc" && record[2] == "def");

            result[1].Is(record => record.Length == 3 && record[1] == "escaped\"\n");

            result[2].Is(record => record.Length == 2);
        }

        [TestMethod]
        public void ExpressionParserExample()
        {
            {
                var parser = Integer.Parser;
                var source = "(1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)";

                parser.Parse(source).WillSucceed(result => result.Value.Is((1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)));
            }

            {
                var parser = Double.Parser;
                var source = "(1 + 2.1 * (3.2 - 4.3) + 5.4 / 6.5) - 7.6 + (8.7 * 9.8)";

                parser.Parse(source).WillSucceed(result => result.Value.Is((1 + 2.1 * (3.2 - 4.3) + 5.4 / 6.5) - 7.6 + (8.7 * 9.8)));
            }

            {
                var parser = IntegerExpression.Parser;
                var source = "(1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)";

                parser.Parse(source).WillSucceed(result => result.Lambda.ToString().Is("() => ((((1 + (2 * (3 - 4))) + (5 / 6)) - 7) + (8 * 9))"));
            }
        }
    }
}
