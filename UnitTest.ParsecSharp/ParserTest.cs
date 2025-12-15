using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp;

public class ParserTest
{
    [Test]
    public async Task AnyTest()
    {
        // Creates a parser that matches any token.
        // This parser only fails if the input is at the end.

        var parser = Any();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task TokenTest()
    {
        // Creates a parser that matches a specified token.
        // Uses `EqualityComparer<T>.Default` for equality comparison.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that matches the token 'a'.
        var parser = Token('a');

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser.Parse(source2).WillFail();
    }

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
    public async Task OneOfTest()
    {
        // Creates a parser that succeeds if the token is included in the specified sequence.

        // Parser that succeeds if the token is one of '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e'.
        var parser = OneOf("6789abcde");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();

        // Overload that takes `params IEnumerable<char>`.
        var parser2 = OneOf('6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e');
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser2.Parse(source2).WillFail();
    }

    [Test]
    public async Task NoneOfTest()
    {
        // Creates a parser that succeeds if the token is not included in the specified sequence.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that succeeds if the token is not one of 'd', 'c', 'b', 'a', '9', '8', '7'.
        var parser = NoneOf("dcba987");

        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        // Overload that takes `params IEnumerable<char>`.
        var parser2 = NoneOf('d', 'c', 'b', 'a', '9', '8', '7');

        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));
    }

    [Test]
    public async Task TakeTest()
    {
        // Creates a parser that reads the specified number of tokens and returns the result as a sequence.

        // Parser that reads 3 tokens.
        var parser = Take(3);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c']));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['1', '2', '3']));

        // If the specified number of tokens exceeds the remaining input, the parser fails.
        var parser2 = Take(9);
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));

        // If 0 is specified, the parser succeeds without consuming input.
        var parser3 = Take(0);
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEmpty());

        // If a value less than 0 is specified, the parser always fails.
        var parser4 = Take(-1);
        await parser4.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));
    }

    [Test]
    public async Task SkipTest()
    {
        // Creates a parser that skips the specified number of tokens.

        var source = "abcdEFGH";

        // Parser that skips 3 tokens and then returns the next token.
        var parser = Skip(3).Right(Any());

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('d'));

        var parser2 = Skip(8).Right(EndOfInput());
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        // If the specified number of tokens cannot be skipped, the parser fails.
        var parser3 = Skip(9);
        await parser3.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));

        // If 0 is specified, the parser succeeds without consuming input.
        var parser4 = Skip(0);
        await parser4.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        // If a value less than 0 is specified, the parser always fails.
        var parser5 = Skip(-1);
        await parser5.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("An input does not have enough length"));
    }

    [Test]
    public async Task TakeWhileTest()
    {
        // Creates a parser that continues to read input as long as the given condition is met.

        // Parser that continues to read input as long as the token is lowercase letter.
        var parser = TakeWhile(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd']));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task TakeWhile1Test()
    {
        // Creates a parser that continues to read input as long as the given condition is met.
        // If no match is found, the parser fails.

        // Parser that continues to read input as long as the token is lowercase letter.
        var parser = TakeWhile1(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd']));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SkipWhileTest()
    {
        // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
        // Works the same as `TakeWhile` but does not collect the result, making it more efficient.

        // Parser that continues to consume input as long as the token is lowercase letter.
        var parser = SkipWhile(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task SkipWhile1Test()
    {
        // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
        // If it cannot skip at least one token, it fails.

        // Parser that continues to consume input as long as the token is lowercase letter.
        var parser = SkipWhile1(char.IsLower);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SatisfyTest()
    {
        // Creates a parser that takes one input and succeeds if the condition is met.
        // Can be used to construct all parsers that consume input.

        var source = "abcdEFGH";

        // Parser that matches 'a'.
        var parser = Satisfy(x => x == 'a'); // == Char('a');
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        // Parser that matches any of 'a', 'b', 'c'.
        var parser2 = Satisfy("abc".Contains); // == OneOf("abc");
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task PureTest()
    {
        // Creates a parser that returns a success result.
        // Used to inject arbitrary values into the parser.
        // This parser does not consume input.

        var parser = Pure("success!");

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("success!"));

        // Delays the generation of the value until the parser is executed.
        var parser2 = Pure(_ => Unit.Instance);
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task FailTest()
    {
        // Creates a parser that returns a failure result.
        // This parser does not consume input.

        var source = "abcdEFGH";

        var parser = Fail<Unit>();
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

        // Overload that allows specifying an error message.
        var parser2 = Fail<Unit>("errormessagetest");
        await parser2.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): errormessagetest"));

        // Can handle the state at the time of parse failure.
        var parser3 = Fail<Unit>(state => $"errormessagetest, current state: '{state}'");
        await parser3.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): errormessagetest, current state: 'a<0x61>'"));
    }

    [Test]
    public async Task AbortTest()
    {
        // Creates a parser that aborts the parsing process when executed.
        // Usually not used directly. Use the `AbortIfEntered` or `AbortWhenFail` combinator.

        // Matches `Abort` or `Any`, but the parsing process ends when `Abort` is evaluated.
        var parser = Abort<char>(_ => "aborted").Or(Any());

        var source = "123456";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("aborted"));
    }

    [Test]
    public async Task GetPositionTest()
    {
        // Creates a parser that retrieves the position of the parse location.
        // This parser does not consume input.

        // Parser that matches `Any` 3 times and then returns the position at that point.
        var parser = Any().Repeat(3).Right(GetPosition());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value.Column).IsEqualTo(4));
    }

    [Test]
    public async Task ChoiceTest()
    {
        // Creates a parser that applies parsers from the beginning and returns the result of the first one that succeeds.
        // If all fail, the last failure is returned as the overall failure.

        // Parser that matches 'c', 'b', or 'a'.
        var parser = Choice(Char('c'), Char('b'), Char('a'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

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

    [Test]
    public async Task TryTest()
    {
        // Creates a parser that executes the parse with parser and returns the value of fallback if it fails, always succeeding.
        // If parser fails to match, it does not consume input.

        // Parser that matches 'a' and returns 'a' if successful, 'x' if it fails.
        var parser = Try(Char('a'), 'x');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('x'));

        // Overload that delays the evaluation of fallback.
        var parser2 = Try(Char('a'), _ => 'x');
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));
    }

    [Test]
    public async Task OptionalTest()
    {
        // Creates a parser that executes the parse with parser and returns a bool indicating whether it matched, always succeeding.
        // If the parser fails to match, it does not consume input.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that matches `Digit`, returns boolean value that matches or not, then matches `Any`.
        var parser = Sequence(Optional(Digit()), Any(), (match, token) => (match, token));

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo((match: false, token: 'a')));

        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo((match: true, token: '2')));

        // Overload that returns a specified default value if it fails.
        var parser2 = Optional(Lower(), '\n');

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('a'));

        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));
    }

    [Test]
    public async Task NotTest()
    {
        // Creates a parser that succeeds if parser fails.
        // This parser does not consume input.

        // Parser that succeeds if the token is not `Lower`.
        var parser = Not(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail();

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task LookAheadTest()
    {
        // Creates a parser that performs a parse with parser without consuming input.

        // Parser that matches `Any` then `Letter` without consuming input, then matches `Any` and concatenates the results.
        var parser = LookAhead(Any() + Letter()) + Any();

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'a']));

        var source2 = "123456";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): At LookAhead -> Parser Failure (Line: 1, Column: 2): Unexpected '2<0x32>'"));
    }

    [Test]
    public async Task ManyTest()
    {
        // Creates a parser that matches parser 0 or more times and returns the result as a sequence.
        // If it does not match even once, the parser returns an empty sequence. In this case, it does not consume input.

        // Parser that matches `Lower` 0 or more times.
        var parser = Many(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd']));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task Many1Test()
    {
        // Creates a parser that matches parser 1 or more times and returns the result as a sequence.
        // If it does not match even once, the parser returns a failure.

        // Parser that matches `Lower` 1 or more times.
        var parser = Many1(Lower());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd']));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SkipManyTest()
    {
        // Creates a parser that matches parser 0 or more times and discards the result.
        // If it does not match even once, it does not consume input.

        // Parser that matches `Lower` 0 or more times, discards the result, then matches `Any`.
        var parser = SkipMany(Lower()).Right(Any());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('E'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));
    }

    [Test]
    public async Task SkipMany1Test()
    {
        // Creates a parser that matches parser 1 or more times and discards the result.

        // Parser that matches `Lower` 1 or more times, discards the result, then matches `Any`.
        var parser = SkipMany1(Lower()).Right(Any());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('E'));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task ManyTillTest()
    {
        // Creates a parser that matches parser repeatedly until terminator is matched and returns the result as a sequence.
        // The result of matching terminator is discarded.

        var source = "abcdEFGH";
        var source2 = "123456";

        // Parser that matches `Any` repeatedly until 'F' is matched.
        var parser = ManyTill(Any(), Char('F'));

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'E']));

        // If terminator is not matched, the parser fails.
        await parser.Parse(source2).WillFail();

        // Use `Many1Till` to ensure that parser matches at least once.
        var parser2 = Many1Till(Letter(), EndOfInput());

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'E', 'F', 'G', 'H']));

        // Since it does not match `Letter`, the parser fails.
        await parser2.Parse(source2).WillFail();
    }

    [Test]
    public async Task SkipTillTest()
    {
        // Creates a parser that repeatedly matches parser until it matches terminator and returns the result of matching terminator.

        // Parser that skips `Lower` until it matches "cd".
        var parser = SkipTill(Lower(), String("cd"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("cd"));

        // Fails because an uppercase letter exists before "cd".
        var source2 = "xyzABcdef";
        await parser.Parse(source2).WillFail();

        // Use `Skip1Till` to ensure that parser matches at least one.
        var parser2 = Skip1Till(Letter(), EndOfInput());
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }

    [Test]
    public async Task TakeTillTest()
    {
        // Creates a parser that reads input until it matches terminator and returns the result as a sequence.
        // The result of matching terminator is discarded. Use `LookAhead` combinator if you do not want to consume it.

        // Parser that reads input until it matches 'E'.
        var parser = TakeTill(Char('E'));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd']));

        // If a terminator that does not match until the end is given, the parser will read the stream to the end,
        // which may affect performance.
        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task MatchTest()
    {
        // Creates a parser that skips until it matches parser and returns the result of matching parser.

        // Parser that skips until it matches "FG".
        var parser = Match(String("FG"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("FG"));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();

        // Parser that skips until it matches `Lower` + `Upper`.
        var parser2 = Match(Sequence(Lower(), Upper())).AsString();

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("dE"));
    }

    [Test]
    public async Task QuotedTest()
    {
        // Creates a parser that retrieves the sequence of tokens between the parsers that match before and after.
        // Can be used to retrieve tokens like string literals.
        // Use the `Quote` extension method if you want to add conditions to the match of the token sequence.

        // Parser that retrieves the string between '<' and '>'.
        var parser = Quoted(Char('<'), Char('>')).AsString();

        var source = "<abcd>";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));

        // Parser that retrieves the string between '<' and '>', and then retrieves the string between them.
        var parser2 = Quoted(parser).AsString();

        var source2 = "<span>test</span>";
        await parser2.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("test"));
    }

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

    [Test]
    public async Task NextTest()
    {
        // A combinator that allows direct description of continuation processing for parser.
        // Since it is processed as a continuation, it can be used to avoid performance degradation by defining self-recursive parsers.

        // Parser that matches `Letter`, returns the result of matching the next `Letter` if successful, and returns '\n' if it fails.
        var parser = Letter().Next(_ => Letter(), '\n');

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('b'));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo('\n'));

        // The overall result is failure because the first `Letter` succeeded but the next `Letter` failed.
        var source3 = "a123";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task GuardTest()
    {
        // Branches success and failure based on a condition for the result matched by parser.

        // Parser that matches a number and succeeds only if it is less than 1000.
        var parser = Many1(DecDigit()).ToInt().Guard(x => x < 1000);

        var source = "123456";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("A value '123456' does not satisfy condition"));

        var source2 = "999";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo(999));

        // If the parser does not match, the validation itself is not performed.
        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task SeparatedByTest()
    {
        // Creates a parser that matches parser repeated 0 or more times separated by separator.
        // The result of matching separator is discarded.

        // [ 1*Number *( "," 1*Number ) ]
        var parser = Many1(Number()).AsString().SeparatedBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123456"]));

        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task SeparatedBy1Test()
    {
        // Creates a parser that matches parser repeated 1 or more times separated by separator.

        // 1*Number *( "," 1*Number )
        var parser = Many1(Number()).AsString().SeparatedBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123456"]));

        var source3 = "abcdEFGH";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task EndByTest()
    {
        // Creates a parser that matches parser repeated 0 or more times with separator at the end.

        // *( 1*Number "," )
        var parser = Many1(Number()).AsString().EndBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEmpty());
    }

    [Test]
    public async Task EndBy1Test()
    {
        // Creates a parser that matches parser repeated 1 or more times with separator at the end.

        // 1*( 1*Number "," )
        var parser = Many1(Number()).AsString().EndBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456"]));

        var source2 = "123456";
        await parser.Parse(source2).WillFail();
    }

    [Test]
    public async Task SeparatedOrEndByTest()
    {
        // Creates a parser that behaves as either `SeparatedBy` or `EndBy`.

        // [ 1*Number *( "," 1*Number ) [ "," ] ]
        var parser = Many1(Number()).AsString().SeparatedOrEndBy(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123456"]));

        var source3 = "123,456,789" + ",";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));
    }

    [Test]
    public async Task SeparatedOrEndBy1Test()
    {
        // Creates a parser that behaves as either `SeparatedBy1` or `EndBy1`.

        // 1*Number *( "," 1*Number ) [ "," ]
        var parser = Many1(Number()).AsString().SeparatedOrEndBy1(Char(','));

        var source = "123,456,789";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));

        var source2 = "123456";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123456"]));

        var source3 = "123,456,789" + ",";
        await parser.Parse(source3).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["123", "456", "789"]));
    }

    [Test]
    public async Task ExceptTest()
    {
        // Creates a parser with exclusion conditions for the specified parser.

        var source = "123456";

        // Parser that matches digits except '5'.
        var parser = Digit().Except(Char('5'));
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo('1'));

        // Parser that matches digits except '5' consecutively and returns the result as a string.
        var parser2 = Many(parser).AsString();
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));
    }

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

    [Test]
    public async Task RepeatTest()
    {
        // Creates a parser that matches parser count times and returns the result as a sequence.

        // 2*( 3*Any )
        var parser = Any().Repeat(3).AsString().Repeat(2);

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["abc", "dEF"]));
    }

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
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcdEFGH"));

        // If you pass `Many(Any())` to the parser, it will match any input until the end, so `close` will match the end of input.
        var parser2 = Many(Any()).Between(Char('"'), Char('"')); // It does not match ( dquote *Any dquote )
        await parser2.Parse("\"abCD1234\"").WillFail(); // `Many(Any())` matches until abCD1234", so `close` does not match " and fails
        // If you want to create a parser that matches this form, consider using `Quoted` or `ManyTill`.
    }

    [Test]
    public async Task QuoteTest()
    {
        // Creates a parser that matches parser repeatedly until it matches the parsers before and after.

        // Parser that matches a string representation that can escape '"' characters.
        var dquoteOrAny = String("\\\"").Map(_ => '\"') | Any();
        var parser = dquoteOrAny.Quote(Char('"')).AsString();

        var source = """
            "abcd\"EFGH"
            """;
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("""abcd"EFGH"""));
    }

    [Test]
    public async Task AppendTest()
    {
        // Matches two parsers in sequence and returns the concatenated result as a sequence.

        var source = "abcdEFGH";

        {
            // 1 character + 1 character
            var parser = Any().Append(Any()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));
            var source2 = "a";
            await parser.Parse(source2).WillFail();
        }

        {
            // Lowercase*n + 1 character
            var parser = Many1(Lower()).Append(Any()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdE"));
            var source2 = "abcd";
            await parser.Parse(source2).WillFail();
        }

        {
            // 1 character + Lowercase*n
            var parser = Any().Append(Many1(Lower())).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
            var source2 = "ABCD";
            await parser.Parse(source2).WillFail();
        }

        {
            // Lowercase*n + Uppercase*n
            var parser = Many1(Lower()).Append(Many1(Upper())).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillFail();
        }

        {
            // array + array
            var parser = Many1(Lower()).ToArray().Append(Many1(Upper()).ToArray()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillFail();
        }

        {
            // string + string
            var parser = Many1(Lower()).AsString().Append(Many1(Upper()).AsString());
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillFail();
        }
    }

    [Test]
    public async Task AppendOperatorTest()
    {
        // `Append` can also be described using operators.

        var source = "abcdEFGH";

        var a = Char('a');
        var b = Char('b');
        var c = Char('c');
        var d = Char('d');

        {
            var appendTwo = a + b;
            await appendTwo.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("ab"));

            var leftAssociative = a + b + c;
            await leftAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abc"));

            var rightAssociative = a + (b + c);
            await rightAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abc"));

            var appendCollection = (a + b) + (c + d);
            await appendCollection.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));
        }

        {
            var ab = a + b as IParser<char, IEnumerable<char>>;
            var bc = b + c as IParser<char, IEnumerable<char>>;
            var cd = c + d as IParser<char, IEnumerable<char>>;

            {
                var leftAssociative = ab + c;
                await leftAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abc"));

                var rightAssociative = a + bc;
                await rightAssociative.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abc"));

                var appendCollection = ab + cd;
                await appendCollection.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo("abcd"));
            }
        }

        {
            var ab = String("ab");
            var cd = String("cd");

            var appendString = ab + cd;
            await appendString.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }
    }

    [Test]
    public async Task AppendOptionalTest()
    {
        // Behaves like Append, but if the right parser fails, it treats the result as an empty sequence and combines them.

        var source = "abcdEFGH";

        {
            // 1 character + 1 character
            var parser = Any().AppendOptional(Any()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("ab"));
            var source2 = "a";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("a"));
        }

        {
            // Lowercase*n + 1 character
            var parser = Many1(Lower()).AppendOptional(Any()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdE"));
            var source2 = "abcd";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }

        {
            // 1 character + Lowercase*n
            var parser = Any().AppendOptional(Many1(Lower())).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
            var source2 = "ABCD";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("A"));
        }

        {
            // Lowercase*n + Uppercase*n
            var parser = Many1(Lower()).AppendOptional(Many1(Upper())).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }

        {
            // array + array
            var parser = Many1(Lower()).ToArray().AppendOptional(Many1(Upper()).ToArray()).AsString();
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }

        {
            // string + string
            var parser = Many1(Lower()).AsString().AppendOptional(Many1(Upper()).AsString());
            await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcdEFGH"));
            var source2 = "abcd";
            await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("abcd"));
        }
    }

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
    public async Task EndTest()
    {
        // A combinator that ensures that parser consumes the input until the end.
        // Fails if it has not reached the end.

        var source = "abcdEFGH";

        // Parser that matches 1 or more lowercase letters and must consume all input at that point.
        var parser = Many1(Lower()).End();
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Expected '<EndOfStream>' but was 'E<0x45>'"));

        // Parser that matches 1 or more lowercase or uppercase letters and must consume all input at that point.
        var parser2 = Many1(Lower() | Upper()).End();
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'E', 'F', 'G', 'H']));
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

        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'E', 'F', 'G', 'H']));

        // In situations where it becomes nested due to using `Many1`, you can use `FoldLeft` instead.
        var parser3 = token.AsEnumerable().FoldLeft((x, y) => x.Concat(y));
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'E', 'F', 'G', 'H']));
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

    [Test]
    public async Task WithConsumeTest()
    {
        // Creates a parser that treats it as a failure if parser succeeds without consuming input.
        // Used when passing a parser that may not consume input, such as `Many`.

        // Parser that avoids infinite loops that occur when `Letter` does not match.
        var parser = Many1(Many(Letter()).WithConsume().AsString());

        var source = "abcdEFGH";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(["abcdEFGH"]));

        var source2 = "123456";
        await parser.Parse(source2).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 1): A parser did not consume any input"));
    }

    [Test]
    public async Task WithMessageTest()
    {
        // Rewrites the error message when parsing fails.

        var parser = Lower().Repeat(6)
            .WithMessage(failure => $"MessageTest Current: '{failure.State.Current}', original message: {failure.Message}");

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 5): MessageTest Current: 'E', original message: Unexpected 'E<0x45>'"));

        var source2 = "abcdefgh";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEquivalentTo(['a', 'b', 'c', 'd', 'e', 'f']));

        var source3 = "123456";
        await parser.Parse(source3).WillFail();
    }

    [Test]
    public async Task AbortWhenFailTest()
    {
        // Aborts the parsing process when parsing fails.

        var parser = Many(Lower().AbortWhenFail(failure => $"Fatal Error! '{failure.State.Current}' is not a lower char!")).AsString()
            .Or(Pure("recovery"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.ToString()).IsEqualTo("Parser Failure (Line: 1, Column: 5): Fatal Error! 'E' is not a lower char!"));
    }

    [Test]
    public async Task AbortIfEnteredTest()
    {
        // Aborts the parsing process if the parser fails after consuming input.
        // Achieves early exit on failure like LL(k) parsers.

        var parser = Sequence(Char('1'), Char('2'), Char('3'), Char('4')).AsString().AbortIfEntered(_ => "abort1234")
            .Or(Pure("recovery"));

        var source = "123456";
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("1234"));

        var source2 = "abcdEFGH";
        await parser.Parse(source2).WillSucceed(async value => await Assert.That(value).IsEqualTo("recovery"));

        var source3 = "123,456,789";
        await parser.Parse(source3).WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("abort1234"));
    }

    [Test]
    public async Task DoTest()
    {
        // Executes the defined action when parsing executes.
        // Can specify actions to be executed on success, or on both success and failure.

        var source = "abcdEFGH";

        // Increases the value of count by 1 when parsing `Lower` succeeds.
        var count = 0;
        var parser = Many(Lower().Do(_ => count++));

        _ = parser.Parse(source);
        await Assert.That(count).IsEqualTo(4);
        _ = parser.Parse(source);
        await Assert.That(count).IsEqualTo(8);

        // Increases the value of success by 1 when parsing `Lower` succeeds, and increases the value of failure by 1 when parsing fails.
        // Connects `Any`, so it parses the source to the end.
        var success = 0;
        var failure = 0;
        var parser2 = Many(Lower().Do(_ => success++, _ => failure++).Or(Any()));

        _ = parser2.Parse(source);
        await Assert.That(success).IsEqualTo(4);
        await Assert.That(failure).IsEqualTo(5);
    }

    [Test]
    public async Task ExceptionTest()
    {
        // If an exception occurs during processing, the parser immediately stops and returns a `Failure` containing the exception.
        // Recovery is not performed for failures due to exceptions. There is no means of recovery.

        // Parser that attempts to return the result of `ToString` on null, and returns "success" if it fails.
        var parser = Pure(null as object).Map(x => x!.ToString()).Or(Pure("success"));

        var source = "abcdEFGH";
        await parser.Parse(source).WillFail(async failure => await Assert.That(failure.Exception.InnerException).IsTypeOf<NullReferenceException>());
    }

    [Test]
    public async Task ParsePartiallyTest()
    {
        // Provides an execution plan that allows you to continue processing without disposing the stream after parsing is complete.

        // Parser that consumes 3 characters.
        var parser = Any().Repeat(3).AsString();

        using var source = StringStream.Create("abcdEFGH");

        var (result, rest) = parser.ParsePartially(source);
        await result.WillSucceed(async value => await Assert.That(value).IsEqualTo("abc"));

        var (result2, rest2) = parser.ParsePartially(rest);
        await result2.WillSucceed(async value => await Assert.That(value).IsEqualTo("dEF"));

        var (result3, rest3) = parser.ParsePartially(rest2);
        await result3.WillFail(async failure => await Assert.That(failure.Message).IsEqualTo("Unexpected '<EndOfStream>'")); // Fails because it reached the end

        // Note that the state at the point of failure is returned.
        await EndOfInput().Parse(rest3).WillSucceed(async value => await Assert.That(value).IsEqualTo(Unit.Instance));
    }
}
