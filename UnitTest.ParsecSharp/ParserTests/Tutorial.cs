using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParsecSharp;
using ParsecSharp.Examples;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.ParserTests;

public class Tutorial
{
    [Test]
    public async Task Usage()
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
                _ = await Assert.That(value).IsEqualTo("aa");
            }

            {
                // gets result value directly, throws an exception when parsing is failed
                var value = result.Value;
                _ = await Assert.That(value).IsEqualTo("aa");
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
                _ = await Assert.That(value).IsEmpty();
            }

            {
                _ = await Assert.That(() => result.Value).ThrowsExactly<ParsecSharpException>().WithMessageContaining("Expected 'bb' but was 'aa'");
            }
        }
    }

    [Test]
    public async Task RegexMatchingExample()
    {
        var source = "aabbb";

        {
            {
                var regex = new Regex("^a*");
                _ = await Assert.That(regex.Match(source).Value).IsEqualTo("aa");

                var parser = Many(Char('a'));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("aa"));
            }

            {
                var regex = new Regex("^b*");
                _ = await Assert.That(regex.Match(source).Value).IsEmpty();

                var parser = Many(Char('b'));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEmpty());
            }
        }

        {
            {
                var regex = new Regex("^a+");
                _ = await Assert.That(regex.Match(source).Value).IsEqualTo("aa");

                var parser = Many1(Char('a'));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("aa"));
            }

            {
                var regex = new Regex("^b+");
                _ = await Assert.That(regex.IsMatch(source)).IsFalse();

                var parser = Many1(Char('b'));
                await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Unexpected 'a<0x61>'"));
            }
        }

        {
            var regex = new Regex("^a*$");
            _ = await Assert.That(regex.IsMatch(source)).IsFalse();

            var parser = Many(Char('a')).End();
            await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 3): Expected '<EndOfStream>' but was 'b<0x62>'"));
        }

        {
            {
                var regex = new Regex("^ab");
                _ = await Assert.That(regex.IsMatch(source)).IsFalse();

                var parser = Sequence(Char('a'), Char('b'));
                await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 2): Unexpected 'a<0x61>'"));

                var parser2 = String("ab");
                await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Expected 'ab' but was 'aa'"));
            }

            {
                var regex = new Regex("ab");
                _ = await Assert.That(regex.Match(source).Value).IsEqualTo("ab");

                var parser = Match(Sequence(Char('a'), Char('b')));
                await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("ab"));

                var parser2 = Match(String("ab"));
                await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));
            }
        }

        {
            var regex = new Regex("^a+b+$");
            _ = await Assert.That(regex.Match(source).Value).IsEqualTo("aabbb");

            var parser = Many1(Char('a')).Append(Many1(Char('b'))).End();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("aabbb"));

            var parser2 = Many1(Char('a')).Append(Many1(Char('b'))).AsString().End();
            await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("aabbb"));
        }

        {
            var regex = new Regex("^a.b");
            _ = await Assert.That(regex.Match(source).Value).IsEqualTo("aab");

            var parser = Sequence(Char('a'), Any(), Char('b'));
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("aab"));
        }
    }

    [Test]
    public async Task LinqQueryNotationExample()
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

            _ = await Assert.That(result.letter).IsEqualTo("A");
            _ = await Assert.That(result.number).IsEqualTo(123);
            _ = await Assert.That(result.flag).IsTrue();
        }
    }

    [Test]
    public async Task JsonParserExample()
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
        _ = await Assert.That(key1).IsEqualTo(123.0);

        var key2 = result?["key2"] as string;
        _ = await Assert.That(key2).IsEqualTo("abc");

        var key3_1 = (bool)result?["key3"]?["key3_1"];
        _ = await Assert.That(key3_1).IsTrue();

        var key3_2 = result?["key3"]?["key3_2"] as IEnumerable<dynamic>;
        _ = await Assert.That(key3_2).IsEquivalentTo((IEnumerable<dynamic>)[1.0, 2.0, 3.0]);

        var key4 = (double)result?["key4"];
        _ = await Assert.That(key4).IsEqualTo(-123.4);
    }

    [Test]
    public async Task CsvParserExample()
    {
        var parser = new CsvParser();

        var source = """
            123,abc,def
            456,"escaped""
            ",xyz
            999,2columns

            """;

        var result = parser.Parse(source).Value.ToArray();

        _ = await Assert.That(result).Count().IsEqualTo(3);

        _ = await Assert.That(result[0][0]).IsEqualTo("123");
        _ = await Assert.That(result[0][1]).IsEqualTo("abc");
        _ = await Assert.That(result[0][2]).IsEqualTo("def");

        _ = await Assert.That(result[1]).Count().IsEqualTo(3);
        _ = await Assert.That(result[1][1]).IsEqualTo("escaped\"\n");

        _ = await Assert.That(result[2]).Count().IsEqualTo(2);
    }

    [Test]
    public async Task ExpressionParserExample()
    {
        {
            var parser = Integer.Parser;
            var source = "(1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)";

            await parser.Parse(source).WillSucceed(async result => await Assert.That(result.Value).IsEqualTo((1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)));
        }

        {
            var parser = Double.Parser;
            var source = "(1 + 2.1 * (3.2 - 4.3) + 5.4 / 6.5) - 7.6 + (8.7 * 9.8)";

            await parser.Parse(source).WillSucceed(async result => await Assert.That(result.Value).IsEqualTo((1 + 2.1 * (3.2 - 4.3) + 5.4 / 6.5) - 7.6 + (8.7 * 9.8)));
        }

        {
            var parser = IntegerExpression.Parser;
            var source = "(1 + 2 * (3 - 4) + 5 / 6) - 7 + (8 * 9)";

            await parser.Parse(source).WillSucceed(async result => await Assert.That(result.Lambda.ToString()).IsEqualTo("() => ((((1 + (2 * (3 - 4))) + (5 / 6)) - 7) + (8 * 9))"));
        }
    }

    [Test]
    public async Task PegParserExample()
    {
        var parser = new PegParser();

        {
            var abcxefg = parser.Parse("Parser <- 'abc' . 'efg'").Value["Parser"];
            await abcxefg.Parse("abcdefg").WillSucceed(async value => await Assert.That(value.Match).IsEqualTo("abcdefg"));
            await abcxefg.Parse("abc_efg_ijk").WillSucceed(async value => await Assert.That(value.Match).IsEqualTo("abc_efg"));
            await abcxefg.Parse("abcdeff").WillFail();
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

            await parsers["Parser1"].Parse("abcdefg").WillSucceed(async value => await Assert.That(value.AllMatches).IsEquivalentTo(["abcdefg", "a", "b", "c", "d", "e", "f", "g"]));
            await parsers["Parser2"].Parse("abcdefg").WillSucceed(async value => await Assert.That(value.AllMatches).IsEquivalentTo(["abcdefg", "abcdefg"]));
            await parsers["Parser3"].Parse("abcdefg").WillSucceed(async value => await Assert.That(value.AllMatches).IsEquivalentTo(["abcdef", "cd", "ef"]));
            await parsers["Parser4"].Parse("abcdefg").WillSucceed(async value => await Assert.That(value.AllMatches).IsEquivalentTo(["abcdef", "abc", "a", "c", "def", "d", "f"]));
        }

        {
            // Parses the PEG of the PEG definition with the PEG parser generated by parsing the PEG of
            // the PEG definition with the PEG parser that generates the PEG parser by parsing the PEG.
            var peg = parser.Parse(PegDefinition).Value;
            var pegParser = peg["Grammar"];

            await pegParser.Parse(PegDefinition).WillSucceed(async value => await Assert.That(value.Match).IsEqualTo(PegDefinition));
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
