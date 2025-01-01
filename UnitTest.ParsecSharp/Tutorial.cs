using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using ParsecSharp.Examples;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp;

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
                _ = ExceptionAssert.Throws<ParsecException>(() =>
                {
                    _ = result.Value;
                    Assert.Fail("does not reach here");
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
                parser.Parse(source).WillSucceed(value => value.Is([]));
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
    public void LinqQueryNotationExample()
    {
        // using linq query notation for sequential parser composition
        {
            var letterParser = Letter().AsString(); // string
            var separator = Char('-'); // char
            var intParser = Many1(DecDigit()).ToInt(); // int
            var booleanParser = CharIgnoreCase('t').Map(_ => true) | CharIgnoreCase('f').Map(_ => false); // bool

            var parser =
                from letter in letterParser
                from _ in separator
                from number in intParser
                from flag in booleanParser
                select new { letter, number, flag };

            var result = parser.Parse("A-123T").Value;

            result.letter.Is("A");
            result.number.Is(123);
            result.flag.IsTrue();
        }
    }

    [TestMethod]
    public void JsonParserExample()
    {
        var parser = new JsonParser();

        // language=json, strict
        var source = """
            {
              "key1": 123,
              "key2": "abc",
              "key3": {
                "key3_1": true,
                "key3_2": [1, 2, 3]
              },
              "key4": -1.234e+2
            }
            """;

        var result = parser.Parse(source)
            .CaseOf(failure => default, success => success.Value);

        var key1 = (double)result?["key1"];
        key1.Is(123.0);

        var key2 = result?["key2"] as string;
        key2.Is("abc");

        var key3_1 = (bool)result?["key3"]?["key3_1"];
        key3_1.IsTrue();

        var key3_2 = result?["key3"]?["key3_2"] as IEnumerable<dynamic>;
        key3_2.Is(1.0, 2.0, 3.0);

        var key4 = (double)result?["key4"];
        key4.Is(-123.4);
    }

    [TestMethod]
    public void CsvParserExample()
    {
        var parser = new CsvParser();

        var source = """
            123,abc,def
            456,"escaped""
            ",xyz
            999,2columns

            """;

        var result = parser.Parse(source).Value.ToArray();

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

    [TestMethod]
    public void PegParserExample()
    {
        var parser = new PegParser();

        {
            var abcxefg = parser.Parse("Parser <- 'abc' . 'efg'").Value["Parser"];
            abcxefg.Parse("abcdefg").WillSucceed(value => value.Match.Is("abcdefg"));
            abcxefg.Parse("abc_efg_ijk").WillSucceed(value => value.Match.Is("abc_efg"));
            abcxefg.Parse("abcdeff").WillFail();
        }

        {
            var peg = """
                Parser1 <- {.}*
                Parser2 <- {.*}
                Parser3 <- . . { . . }+
                Parser4 <- {Parser5}+
                Parser5 <- {.} . {.}
                """;

            var parsers = parser.Parse(peg).Value;

            parsers["Parser1"].Parse("abcdefg").WillSucceed(value => value.AllMatches.Is("abcdefg", "a", "b", "c", "d", "e", "f", "g"));
            parsers["Parser2"].Parse("abcdefg").WillSucceed(value => value.AllMatches.Is("abcdefg", "abcdefg"));
            parsers["Parser3"].Parse("abcdefg").WillSucceed(value => value.AllMatches.Is("abcdef", "cd", "ef"));
            parsers["Parser4"].Parse("abcdefg").WillSucceed(value => value.AllMatches.Is("abcdef", "abc", "a", "c", "def", "d", "f"));
        }

        {
            // Parses the PEG of the PEG definition with the PEG parser generated by parsing the PEG of
            // the PEG definition with the PEG parser that generates the PEG parser by parsing the PEG.
            var peg = parser.Parse(PegDefinition).Value;
            var pegParser = peg["Grammar"];

            pegParser.Parse(PegDefinition).WillSucceed(value => value.Match.Is(PegDefinition));
        }
    }

    // https://bford.info/pub/lang/peg.pdf
    private const string PegDefinition = """
        # Hierarchical syntax
        Grammar    <- Spacing Definition+ EndOfFile
        Definition <- Identifier LEFTARROW Expression

        Expression <- Sequence (SLASH Sequence)*
        Sequence   <- Prefix*
        Prefix     <- (AND / NOT)? Suffix
        Suffix     <- Primary (QUESTION / STAR / PLUS)?
        Primary    <- Identifier !LEFTARROW
                    / OPEN Expression CLOSE
                    / Literal / Class / DOT

        # Lexical syntax
        Identifier <- IdentStart IdentCont* Spacing
        IdentStart <- [a-zA-Z_]
        IdentCont  <- IdentStart / [0-9]

        Literal    <- ['] (!['] Char)* ['] Spacing
                    / ["] (!["] Char)* ["] Spacing
        Class      <- '[' (!']' Range)* ']' Spacing
        Range      <- Char '-' Char / Char
        Char       <- '\\' [nrt'"\[\]\\]
                    / '\\' [0-2][0-7][0-7]
                    / '\\' [0-7][0-7]?
                    / !'\\' .

        LEFTARROW  <- '<-' Spacing
        SLASH      <- '/' Spacing
        AND        <- '&' Spacing
        NOT        <- '!' Spacing
        QUESTION   <- '?' Spacing
        STAR       <- '*' Spacing
        PLUS       <- '+' Spacing
        OPEN       <- '(' Spacing
        CLOSE      <- ')' Spacing
        DOT        <- '.' Spacing

        Spacing    <- (Space / Comment)*
        Comment    <- '#' (!EndOfLine .)* EndOfLine
        Space      <- ' ' / '\t' / EndOfLine
        EndOfLine  <- '\r\n' / '\n' / '\r'
        EndOfFile  <- !.

        """;
}
