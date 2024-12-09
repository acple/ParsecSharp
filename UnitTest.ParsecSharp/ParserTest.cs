using System;
using System.Linq;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class ParserTest
    {
        private const string _abcdEFGH = "abcdEFGH";

        private const string _123456 = "123456";

        private const string _commanum = "123,456,789";

        [TestMethod]
        public void AnyTest()
        {
            // Creates a parser that matches any token.
            // This parser only fails if the input is at the end.

            var parser = Any();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void TokenTest()
        {
            // Creates a parser that matches a specified token.
            // Uses `EqualityComparer<T>.Default` for equality comparison.

            var source = _abcdEFGH;
            var source2 = _123456;

            // Parser that matches the token 'a'.
            var parser = Token('a');

            parser.Parse(source).WillSucceed(value => value.Is('a'));

            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void EndOfInputTest()
        {
            // Creates a parser that matches the end of the input.

            var parser = EndOfInput();

            var source = string.Empty;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void NullTest()
        {
            // Creates a parser that matches an empty string and always succeeds in any state.
            // This parser does not consume input.

            var parser = Null();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = string.Empty;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void OneOfTest()
        {
            // Creates a parser that succeeds if the token is included in the specified sequence.

            // Parser that succeeds if the token is one of '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e'.
            var parser = OneOf("6789abcde");

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();

            // Overload that takes `params IEnumerable<char>`.
            var parser2 = OneOf('6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e');
            parser2.Parse(source).WillSucceed(value => value.Is('a'));

            parser2.Parse(source2).WillFail();
        }

        [TestMethod]
        public void NoneOfTest()
        {
            // Creates a parser that succeeds if the token is not included in the specified sequence.

            var source = _abcdEFGH;
            var source2 = _123456;

            // Parser that succeeds if the token is not one of 'd', 'c', 'b', 'a', '9', '8', '7'.
            var parser = NoneOf("dcba987");

            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            parser.Parse(source2).WillSucceed(value => value.Is('1'));

            // Overload that takes `params IEnumerable<char>`.
            var parser2 = NoneOf('d', 'c', 'b', 'a', '9', '8', '7');

            parser2.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            parser2.Parse(source2).WillSucceed(value => value.Is('1'));
        }

        [TestMethod]
        public void TakeTest()
        {
            // Creates a parser that reads the specified number of tokens and returns the result as a sequence.

            // Parser that reads 3 tokens.
            var parser = Take(3);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1', '2', '3'));

            // If the specified number of tokens exceeds the remaining input, the parser fails.
            var parser2 = Take(9);
            parser2.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));

            // If 0 is specified, the parser succeeds without consuming input.
            var parser3 = Take(0);
            parser3.Parse(source).WillSucceed(value => value.Is([]));

            // If a value less than 0 is specified, the parser always fails.
            var parser4 = Take(-1);
            parser4.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));
        }

        [TestMethod]
        public void SkipTest()
        {
            // Creates a parser that skips the specified number of tokens.

            var source = _abcdEFGH;

            // Parser that skips 3 tokens and then returns the next token.
            var parser = Skip(3).Right(Any());

            parser.Parse(source).WillSucceed(value => value.Is('d'));

            var parser2 = Skip(8).Right(EndOfInput());
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            // If the specified number of tokens cannot be skipped, the parser fails.
            var parser3 = Skip(9);
            parser3.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));

            // If 0 is specified, the parser succeeds without consuming input.
            var parser4 = Skip(0);
            parser4.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            // If a value less than 0 is specified, the parser always fails.
            var parser5 = Skip(-1);
            parser5.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));
        }

        [TestMethod]
        public void TakeWhileTest()
        {
            // Creates a parser that continues to read input as long as the given condition is met.

            // Parser that continues to read input as long as the token is lowercase letter.
            var parser = TakeWhile(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Enumerable.Empty<char>()));
        }

        [TestMethod]
        public void TakeWhile1Test()
        {
            // Creates a parser that continues to read input as long as the given condition is met.
            // If no match is found, the parser fails.

            // Parser that continues to read input as long as the token is lowercase letter.
            var parser = TakeWhile1(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipWhileTest()
        {
            // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
            // Works the same as `TakeWhile` but does not collect the result, making it more efficient.

            // Parser that continues to consume input as long as the token is lowercase letter.
            var parser = SkipWhile(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void SkipWhile1Test()
        {
            // Creates a parser that continues to consume input as long as the given condition is met and discards the result.
            // If it cannot skip at least one token, it fails.

            // Parser that continues to consume input as long as the token is lowercase letter.
            var parser = SkipWhile1(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SatisfyTest()
        {
            // Creates a parser that takes one input and succeeds if the condition is met.
            // Can be used to construct all parsers that consume input.

            var source = _abcdEFGH;

            // Parser that matches 'a'.
            var parser = Satisfy(x => x == 'a'); // == Char('a');
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            // Parser that matches any of 'a', 'b', 'c'.
            var parser2 = Satisfy("abc".Contains); // == OneOf("abc");
            parser2.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void PureTest()
        {
            // Creates a parser that returns a success result.
            // Used to inject arbitrary values into the parser.
            // This parser does not consume input.

            var parser = Pure("success!");

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("success!"));

            // Delays the generation of the value until the parser is executed.
            var parser2 = Pure(_ => Unit.Instance);
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void FailTest()
        {
            // Creates a parser that returns a failure result.
            // This parser does not consume input.

            var source = _abcdEFGH;

            var parser = Fail<Unit>();
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            // Overload that allows specifying an error message.
            var parser2 = Fail<Unit>("errormessagetest");
            parser2.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): errormessagetest"));

            // Can handle the state at the time of parse failure.
            var parser3 = Fail<Unit>(state => $"errormessagetest, current state: '{state}'");
            parser3.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): errormessagetest, current state: 'a<0x61>'"));
        }

        [TestMethod]
        public void AbortTest()
        {
            // Creates a parser that aborts the parsing process when executed.
            // Usually not used directly. Use the `AbortIfEntered` or `AbortWhenFail` combinator.

            // Matches `Abort` or `Any`, but the parsing process ends when `Abort` is evaluated.
            var parser = Abort<char>(_ => "aborted").Or(Any());

            var source = _123456;
            parser.Parse(source).WillFail(failure => failure.Message.Is("aborted"));
        }

        [TestMethod]
        public void GetPositionTest()
        {
            // Creates a parser that retrieves the position of the parse location.
            // This parser does not consume input.

            // Parser that matches `Any` 3 times and then returns the position at that point.
            var parser = Any().Repeat(3).Right(GetPosition());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Column.Is(4));
        }

        [TestMethod]
        public void ChoiceTest()
        {
            // Creates a parser that applies parsers from the beginning and returns the result of the first one that succeeds.
            // If all fail, the last failure is returned as the overall failure.

            // Parser that matches 'c', 'b', or 'a'.
            var parser = Choice(Char('c'), Char('b'), Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SequenceTest()
        {
            // Creates a parser that matches parsers in sequence and returns the concatenated result as a sequence.

            // Parser that matches 'a' + 'b' + 'c' + 'd' and converts to "abcd".
            var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).AsString();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("abcd"));

            var source2 = "abCDEF";
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 3): Unexpected 'C<0x43>'"));
        }

        [TestMethod]
        public void TryTest()
        {
            // Creates a parser that executes the parse with parser and returns the value of resume if it fails, always succeeding.
            // If parser fails to match, it does not consume input.

            // Parser that matches 'a' and returns 'a' if successful, 'x' if it fails.
            var parser = Try(Char('a'), 'x');

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('x'));

            // Overload that delays the evaluation of resume.
            var parser2 = Try(Char('a'), _ => 'x');
            parser2.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void OptionalTest()
        {
            // Creates a parser that executes the parse with parser and returns a bool indicating whether it matched, always succeeding.
            // If the parser fails to match, it does not consume input.

            var source = _abcdEFGH;
            var source2 = _123456;

            // Parser that matches `Digit`, returns boolean value that matches or not, then matches `Any`.
            var parser = Optional(Digit()).Right(Any());

            parser.Parse(source).WillSucceed(value => value.Is('a'));

            parser.Parse(source2).WillSucceed(value => value.Is('2'));

            // Overload that returns a specified default value if it fails.
            var parser2 = Optional(Lower(), '\n');

            parser2.Parse(source).WillSucceed(value => value.Is('a'));

            parser2.Parse(source2).WillSucceed(value => value.Is('\n'));
        }

        [TestMethod]
        public void NotTest()
        {
            // Creates a parser that succeeds if parser fails.
            // This parser does not consume input.

            // Parser that succeeds if the token is not `Lower`.
            var parser = Not(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillFail();

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void LookAheadTest()
        {
            // Creates a parser that performs a parse with parser without consuming input.

            // Parser that matches `Any` then `Letter` without consuming input, then matches `Any` and concatenates the results.
            var parser = LookAhead(Any().Right(Letter())).Append(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b', 'a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): At LookAhead -> Parser Failure (Line: 1, Column: 2): Unexpected '2<0x32>'"));
        }

        [TestMethod]
        public void ManyTest()
        {
            // Creates a parser that matches parser 0 or more times and returns the result as a sequence.
            // If it does not match even once, the parser returns an empty sequence. In this case, it does not consume input.

            // Parser that matches `Lower` 0 or more times.
            var parser = Many(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void Many1Test()
        {
            // Creates a parser that matches parser 1 or more times and returns the result as a sequence.
            // If it does not match even once, the parser returns a failure.

            // Parser that matches `Lower` 1 or more times.
            var parser = Many1(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipManyTest()
        {
            // Creates a parser that matches parser 0 or more times and discards the result.
            // If it does not match even once, it does not consume input.

            // Parser that matches `Lower` 0 or more times, discards the result, then matches `Any`.
            var parser = SkipMany(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1'));
        }

        [TestMethod]
        public void SkipMany1Test()
        {
            // Creates a parser that matches parser 1 or more times and discards the result.

            // Parser that matches `Lower` 1 or more times, discards the result, then matches `Any`.
            var parser = SkipMany1(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void ManyTillTest()
        {
            // Creates a parser that matches parser repeatedly until terminator is matched and returns the result as a sequence.
            // The result of matching terminator is discarded.

            var source = _abcdEFGH;
            var source2 = _123456;

            // Parser that matches `Any` repeatedly until 'F' is matched.
            var parser = ManyTill(Any(), Char('F'));

            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E'));

            // If terminator is not matched, the parser fails.
            parser.Parse(source2).WillFail();

            // Use `Many1Till` to ensure that parser matches at least once.
            var parser2 = Many1Till(Letter(), EndOfInput());

            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));

            // Since it does not match `Letter`, the parser fails.
            parser2.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipTillTest()
        {
            // Creates a parser that repeatedly matches parser until it matches terminator and returns the result of matching terminator.

            // Parser that skips `Lower` until it matches "cd".
            var parser = SkipTill(Lower(), String("cd"));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("cd"));

            // Fails because an uppercase letter exists before "cd".
            var source2 = "xyzABcdef";
            parser.Parse(source2).WillFail();

            // Use `Skip1Till` to ensure that parser matches at least once.
            var parser2 = Skip1Till(Letter(), EndOfInput());
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void TakeTillTest()
        {
            // Creates a parser that reads input until it matches terminator and returns the result as a sequence.
            // The result of matching terminator is discarded. Use `LookAhead` combinator if you do not want to consume it.

            // Parser that reads input until it matches 'E'.
            var parser = TakeTill(Char('E'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            // If a terminator that does not match until the end is given, the parser will read the stream to the end,
            // which may affect performance.
            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void MatchTest()
        {
            // Creates a parser that skips until it matches parser and returns the result of matching parser.

            // Parser that skips until it matches "FG".
            var parser = Match(String("FG"));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("FG"));

            var source2 = _123456;
            parser.Parse(source2).WillFail();

            // Parser that skips until it matches `Lower` + `Upper`.
            var parser2 = Match(Sequence(Lower(), Upper())).AsString();

            parser2.Parse(source).WillSucceed(value => value.Is("dE"));
        }

        [TestMethod]
        public void QuotedTest()
        {
            // Creates a parser that retrieves the sequence of tokens between the parsers that match before and after.
            // Can be used to retrieve tokens like strings.
            // Use the `Quote` extension method if you want to add conditions to the match of the token sequence.

            // Parser that retrieves the string between '<' and '>'.
            var parser = Quoted(Char('<'), Char('>')).AsString();

            var source = "<abcd>";
            parser.Parse(source).WillSucceed(value => value.Is("abcd"));

            // Parser that retrieves the string between '<' and '>', and then retrieves the string between them.
            var parser2 = Quoted(parser).AsString();

            var source2 = "<span>test</span>";
            parser2.Parse(source2).WillSucceed(value => value.Is("test"));
        }

        [TestMethod]
        public void AtomTest()
        {
            // Treats the specified parser as the smallest unit of the parser.
            // Even if the parsing process fails halfway, it backtracks to the starting point.
            // Use in combination with `WithConsume` / `AbortIfEntered`.

            var abCD = Sequence(Char('a'), Char('b'), Char('C'), Char('D'));
            var parser = Atom(abCD);

            var source = _abcdEFGH;
            abCD.Parse(source).WillFail(failure => failure.State.Position.Is(position => position.Line == 1 && position.Column == 3));
            parser.Parse(source).WillFail(failure => failure.State.Position.Is(position => position.Line == 1 && position.Column == 1));
        }

        [TestMethod]
        public void DelayTest()
        {
            // Delays the construction of the specified parser until the parsing execution.
            // Can also be used as a reference holder, essential for forward references and recursion.

            // Parser that matches 'a'. However, it is constructed at the time of parsing execution.
            var parser = Delay(() => Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            // The `Func<T>` passed to `Delay` is executed only once at the time of parsing execution.
            // Therefore, be careful as it may behave unexpectedly if the process contains side effects.
            // Reusing this parser object can improve performance.
        }

        [TestMethod]
        public void FixTest()
        {
            // A helper combinator for constructing self-recursive parsers in local variables or parser definition expressions.
            // Due to the specifications of C#, it is necessary to provide type arguments when used alone due to lack of type information.

            // Parser that matches a character enclosed in any number of "{}".
            var parser = Fix<char>(self => self.Or(Any()).Between(Char('{'), Char('}')));

            var source = "{{{{{*}}}}}";
            parser.Parse(source).WillSucceed(value => value.Is('*'));

            // Overload that takes parameters. Allows flexible description of recursive parsers.
            // Famous palindrome parser. S ::= "a" S "a" | "b" S "b" | ""
            var parser2 = Fix<IParser<char, Unit>, Unit>((self, rest) =>
                Char('a').Right(self(Char('a').Right(rest))) | Char('b').Right(self(Char('b').Right(rest))) | rest);

            var source2 = "abbaabba";
            parser2(EndOfInput()).Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void NextTest()
        {
            // A combinator that allows direct description of continuation processing for parser.
            // Since it is processed as a continuation, it can be used to avoid performance degradation by defining self-recursive parsers.

            // Parser that matches `Letter`, returns the result of matching the next `Letter` if successful, and returns '\n' if it fails.
            var parser = Letter().Next(_ => Letter(), '\n');

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('\n'));

            // The overall result is failure because the first `Letter` succeeded but the next `Letter` failed.
            var source3 = "a123";
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void GuardTest()
        {
            // Branches success and failure based on a condition for the result matched by parser.

            // Parser that matches a number and succeeds only if it is less than 1000.
            var parser = Many1(DecDigit()).ToInt().Guard(x => x < 1000);

            var source = _123456;
            parser.Parse(source).WillFail(failure => failure.Message.Is("A value '123456' does not satisfy condition"));

            var source2 = "999";
            parser.Parse(source2).WillSucceed(value => value.Is(999));

            // If the parser does not match, the validation itself is not performed.
            var source3 = _abcdEFGH;
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void SeparatedByTest()
        {
            // Creates a parser that matches parser repeated 0 or more times separated by separator.
            // The result of matching separator is discarded.

            // [ 1*Number *( "," 1*Number ) ]
            var parser = Many1(Number()).AsString().SeparatedBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void SeparatedBy1Test()
        {
            // Creates a parser that matches parser repeated 1 or more times separated by separator.

            // 1*Number *( "," 1*Number )
            var parser = Many1(Number()).AsString().SeparatedBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void EndByTest()
        {
            // Creates a parser that matches parser repeated 0 or more times with separator at the end.

            // *( 1*Number "," )
            var parser = Many1(Number()).AsString().EndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void EndBy1Test()
        {
            // Creates a parser that matches parser repeated 1 or more times with separator at the end.

            // 1*( 1*Number "," )
            var parser = Many1(Number()).AsString().EndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SeparatedOrEndByTest()
        {
            // Creates a parser that behaves as either `SeparatedBy` or `EndBy`.

            // [ 1*Number *( "," 1*Number ) [ "," ] ]
            var parser = Many1(Number()).AsString().SeparatedOrEndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).WillSucceed(value => value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void SeparatedOrEndBy1Test()
        {
            // Creates a parser that behaves as either `SeparatedBy1` or `EndBy1`.

            // 1*Number *( "," 1*Number ) [ "," ]
            var parser = Many1(Number()).AsString().SeparatedOrEndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).WillSucceed(value => value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void ExceptTest()
        {
            // Creates a parser with exclusion conditions for the specified parser.

            var source = _123456;

            // Parser that matches digits except '5'.
            var parser = Digit().Except(Char('5'));
            parser.Parse(source).WillSucceed(value => value.Is('1'));

            // Parser that matches digits except '5' consecutively and returns the result as a string.
            var parser2 = Many(parser).AsString();
            parser2.Parse(source).WillSucceed(value => value.Is("1234"));
        }

        [TestMethod]
        public void ChainTest()
        {
            // Creates a recursive parser that starts with a single parser, creates the next parser based on the result, and repeats until it fails.

            // Parser that matches any character consecutively, and returns the matched character and its count as a result.
            var parser = Any().Map(x => (x, count: 1))
                .Chain(match => Char(match.x).Map(_ => (match.x, match.count + 1)))
                .Map(match => match.x.ToString() + match.count.ToString());

            var source = "aaaaaaaaa";
            parser.Parse(source).WillSucceed(value => value.Is("a9"));

            var source2 = "aaabbbbcccccdddddd";
            Many(parser).Join().Parse(source2).WillSucceed(value => value.Is("a3b4c5d6"));

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

            Expr2().Parse("1+2+3+4").Value.Is(1 + 2 + 3 + 4);
        }

        [TestMethod]
        public void ChainLeftTest()
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
            parser.Parse(source).WillSucceed(value => value.Is(((10 + 5) - 3) + 1));

            var source2 = "100-20-5+50";
            parser.Parse(source2).WillSucceed(value => value.Is(((100 - 20) - 5) + 50));

            var source3 = "123";
            parser.Parse(source3).WillSucceed(value => value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).WillFail();

            var source5 = "1-2+3+ABCD";
            parser.Parse(source5).WillSucceed(value => value.Is((1 - 2) + 3));
            parser.Right(Any()).Parse(source5).WillSucceed(value => value.Is('+'));
        }

        [TestMethod]
        public void ChainRightTest()
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
            parser.Parse(source).WillSucceed(value => value.Is(10 + (5 - (3 + 1))));

            var source2 = "100-20-5+50";
            parser.Parse(source2).WillSucceed(value => value.Is(100 - (20 - (5 + 50))));

            var source3 = "123";
            parser.Parse(source3).WillSucceed(value => value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).WillFail();

            var source5 = "1-2+3+ABCD";
            parser.Parse(source5).WillSucceed(value => value.Is(1 - (2 + 3)));
            parser.Right(Any()).Parse(source5).WillSucceed(value => value.Is('+'));
        }

        [TestMethod]
        public void FoldLeftTest()
        {
            // Takes an initial value and an aggregation function as arguments, and creates a parser that aggregates the parsed results from left to right.

            // Parser that matches 0 or more digits, and repeatedly applies (x => accumulator - x) to the initial value 10 from the left.
            var parser = Digit().AsString().ToInt().FoldLeft(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).WillSucceed(value => value.Is(((((10 - 1) - 2) - 3) - 4) - 5));

            // Overload that does not use an initial value.
            var parser2 = Digit().AsString().ToInt().FoldLeft((x, y) => x - y);
            parser2.Parse(source).WillSucceed(value => value.Is((((1 - 2) - 3) - 4) - 5));
        }

        [TestMethod]
        public void FoldRightTest()
        {
            // Takes an initial value and an aggregation function as arguments, and creates a parser that aggregates the parsed results from right to left.

            // Parser that matches 0 or more digits, and repeatedly applies (x => x - accumulator) to the initial value 10 from the right.
            var parser = Digit().AsString().ToInt().FoldRight(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).WillSucceed(value => value.Is(1 - (2 - (3 - (4 - (5 - 10))))));

            // Overload that does not use an initial value.
            var parser2 = Digit().AsString().ToInt().FoldRight((x, y) => x - y);
            parser2.Parse(source).WillSucceed(value => value.Is(1 - (2 - (3 - (4 - 5)))));
        }

        [TestMethod]
        public void RepeatTest()
        {
            // Creates a parser that matches parser count times and returns the result as a sequence.

            // 2*( 3*Any )
            var parser = Any().Repeat(3).AsString().Repeat(2);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("abc", "dEF"));
        }

        [TestMethod]
        public void LeftTest()
        {
            // Creates a parser that matches two parsers in sequence and returns the result of left, discarding the result of right.

            // Parser that matches ( "a" "b" ) and returns 'a'.
            var parser = Char('a').Left(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = "a";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void RightTest()
        {
            // Creates a parser that matches two parsers in sequence and returns the result of right, discarding the result of left.

            // Parser that matches ( "a" "b" ) and returns 'b'.
            var parser = Char('a').Right(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b'));

            var source2 = "b";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void BetweenTest()
        {
            // Creates a parser that matches parser enclosed by open and close.
            // The results of open and close are discarded, and only the result of the middle parser is returned.

            // Parser that matches 1 or more letters enclosed in "[]".
            // ( "[" 1*Letter "]" )
            var parser = Many1(Letter()).Between(Char('['), Char(']'));

            var source = $"[{_abcdEFGH}]";
            parser.Parse(source).WillSucceed(value => value.Is(_abcdEFGH));

            // If you pass `Many(Any())` to the parser, it will match any input until the end, so `close` will match the end of input.
            var parser2 = Many(Any()).Between(Char('"'), Char('"')); // It does not match ( dquote *Any dquote )
            parser2.Parse(@"""abCD1234""").WillFail(); // `Many(Any())` matches until abCD1234", so `close` does not match " and fails
            // If you want to create a parser that matches this form, consider using `Quoted` or `ManyTill`.
        }

        [TestMethod]
        public void QuoteTest()
        {
            // Creates a parser that matches parser repeatedly until it matches the parsers before and after.

            // Parser that matches a string representation that can escape '"' characters.
            var dquoteOrAny = String("\\\"").Map(_ => '\"') | Any();
            var parser = dquoteOrAny.Quote(Char('"')).AsString();

            var source = "\"abcd\\\"EFGH\"";
            parser.Parse(source).WillSucceed(value => value.Is("abcd\"EFGH"));
        }

        [TestMethod]
        public void AppendTest()
        {
            // Matches two parsers in sequence and returns the concatenated result as a sequence.

            var source = _abcdEFGH;

            {
                // 1 character + 1 character
                var parser = Any().Append(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("ab"));
                var source2 = "a";
                parser.Parse(source2).WillFail();
            }

            {
                // Lowercase*n + 1 character
                var parser = Many1(Lower()).Append(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdE"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }

            {
                // 1 character + Lowercase*n
                var parser = Any().Append(Many1(Lower())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcd"));
                var source2 = "ABCD";
                parser.Parse(source2).WillFail();
            }

            {
                // Lowercase*n + Uppercase*n
                var parser = Many1(Lower()).Append(Many1(Upper())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }

            {
                // array + array
                var parser = Many1(Lower()).ToArray().Append(Many1(Upper()).ToArray()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }

            {
                // string + string
                var parser = Many1(Lower()).AsString().Append(Many1(Upper()).AsString());
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }
        }

        [TestMethod]
        public void AppendOptionalTest()
        {
            // Behaves like Append, but if the right parser fails, it treats the result as an empty sequence and combines them.

            var source = _abcdEFGH;

            {
                // 1 character + 1 character
                var parser = Any().AppendOptional(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("ab"));
                var source2 = "a";
                parser.Parse(source2).WillSucceed(value => value.Is("a"));
            }

            {
                // Lowercase*n + 1 character
                var parser = Many1(Lower()).AppendOptional(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdE"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }

            {
                // 1 character + Lowercase*n
                var parser = Any().AppendOptional(Many1(Lower())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcd"));
                var source2 = "ABCD";
                parser.Parse(source2).WillSucceed(value => value.Is("A"));
            }

            {
                // Lowercase*n + Uppercase*n
                var parser = Many1(Lower()).AppendOptional(Many1(Upper())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }

            {
                // array + array
                var parser = Many1(Lower()).ToArray().AppendOptional(Many1(Upper()).ToArray()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }

            {
                // string + string
                var parser = Many1(Lower()).AsString().AppendOptional(Many1(Upper()).AsString());
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }
        }

        [TestMethod]
        public void IgnoreTest()
        {
            // Discards the parsed result.
            // Used when you want to match the type or explicitly discard the value.

            // Parser that matches 1 or more lowercase letters and discards the result.
            var parser = Many1(Lower()).Ignore();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            parser.Right(Any()).Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void EndTest()
        {
            // A combinator that ensures that parser consumes the input until the end.
            // Fails if it has not reached the end.

            var source = _abcdEFGH;

            // Parser that matches 1 or more lowercase letters and must consume all input at that point.
            var parser = Many1(Lower()).End();
            parser.Parse(source).WillFail(failure => failure.Message.Is("Expected '<EndOfStream>' but was 'E<0x45>'"));

            // Parser that matches 1 or more lowercase or uppercase letters and must consume all input at that point.
            var parser2 = Many1(Lower() | Upper()).End();
            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));
        }

        [TestMethod]
        public void FlattenTest()
        {
            // A combinator that flattens a parser that results in a nested `IEnumerable<T>`.

            var source = _abcdEFGH;

            // Parser that takes 2 tokens.
            var token = Any().Repeat(2);
            // Parser that matches the parser that takes 2 tokens 1 or more times.
            var parser = Many1(token);
            // Parser that flattens the result of parser.
            var parser2 = parser.Flatten();

            parser.Parse(source).WillSucceed(value => value.Is(value => value.Count == 4 && value.All(x => x.Count == 2)));

            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));

            // In situations where it becomes nested due to using `Many1`, consider using `FoldLeft` instead.
            var parser3 = token.FoldLeft((x, y) => [.. x, .. y]);
            parser3.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));
        }

        [TestMethod]
        public void SingletonTest()
        {
            // A combinator that returns the result matched by parser as a single-element sequence.

            // Parser that matches 'a' and returns [ 'a' ].
            var parser = Char('a').Singleton();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Count.Is(1));
        }

        [TestMethod]
        public void WithConsumeTest()
        {
            // Creates a parser that treats it as a failure if parser succeeds without consuming input.
            // Used when passing a parser that may not consume input, such as `Many`.

            // Parser that avoids infinite loops that occur when `Letter` does not match.
            var parser = Many1(Many(Letter()).WithConsume().AsString());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(_abcdEFGH));

            var source2 = _123456;
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): A parser did not consume any input"));
        }

        [TestMethod]
        public void WithMessageTest()
        {
            // Rewrites the error message when parsing fails.

            var parser = Many1(Digit())
                .WithMessage(failure => $"MessageTest Current: '{failure.State.Current}', original message: {failure.Message}");

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): MessageTest Current: 'a', original message: Unexpected 'a<0x61>'"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1', '2', '3', '4', '5', '6'));
        }

        [TestMethod]
        public void AbortWhenFailTest()
        {
            // Aborts the parsing process when parsing fails.

            var parser = Many(Lower().AbortWhenFail(failure => $"Fatal Error! '{failure.State.Current}' is not a lower char!")).AsString()
                .Or(Pure("recovery"));

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 5): Fatal Error! 'E' is not a lower char!"));
        }

        [TestMethod]
        public void AbortIfEnteredTest()
        {
            // Aborts the parsing process if the parser fails after consuming input.
            // Achieves early exit on failure like LL(k) parsers.

            var parser = Sequence(Char('1'), Char('2'), Char('3'), Char('4')).AsString().AbortIfEntered(_ => "abort1234")
                .Or(Pure("recovery"));

            var source = _123456;
            parser.Parse(source).WillSucceed(value => value.Is("1234"));

            var source2 = _abcdEFGH;
            parser.Parse(source2).WillSucceed(value => value.Is("recovery"));

            var source3 = _commanum;
            parser.Parse(source3).WillFail(failure => failure.Message.Is("abort1234")); // Fails because it consumed input up to 123 and failed, so recovery is not performed
        }

        [TestMethod]
        public void DoTest()
        {
            // Executes the defined action when parsing executes.
            // Can specify actions to be executed on success, or on both success and failure.

            var source = _abcdEFGH;

            // Increases the value of count by 1 when parsing `Lower` succeeds.
            var count = 0;
            var parser = Many(Lower().Do(_ => count++));

            _ = parser.Parse(source);
            count.Is(4);
            _ = parser.Parse(source);
            count.Is(8);

            // Increases the value of success by 1 when parsing `Lower` succeeds, and increases the value of failure by 1 when parsing fails.
            // Connects `Any`, so it parses the source to the end.
            var success = 0;
            var failure = 0;
            var parser2 = Many(Lower().Do(_ => success++, _ => failure++).Or(Any()));

            _ = parser2.Parse(source);
            success.Is(4);
            failure.Is(5);
        }

        [TestMethod]
        public void ExceptionTest()
        {
            // If an exception occurs during processing, the parser immediately stops and returns a `Failure` containing the exception.
            // Recovery is not performed for failures due to exceptions. There is no means of recovery.

            // Parser that attempts to return the result of `ToString` on null, and returns "success" if it fails.
            var parser = Pure(null as object).Map(x => x!.ToString()!).Or(Pure("success"));

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.Exception.InnerException!.GetType().Name.Is(nameof(NullReferenceException)));
        }

        [TestMethod]
        public void ParsePartiallyTest()
        {
            // Provides an execution plan that allows you to continue processing without discarding the stream after parsing is complete.

            // Parser that consumes 3 characters.
            var parser = Any().Repeat(3).AsString();

            using var source = StringStream.Create(_abcdEFGH);

            var (result, rest) = parser.ParsePartially(source);
            result.WillSucceed(value => value.Is("abc"));

            var (result2, rest2) = parser.ParsePartially(rest);
            result2.WillSucceed(value => value.Is("dEF"));

            var (result3, rest3) = parser.ParsePartially(rest2);
            result3.WillFail(failure => failure.Message.Is("Unexpected '<EndOfStream>'")); // Fails because it reached the end

            // Note that the state at the point of failure is returned.
            EndOfInput().Parse(rest3).WillSucceed(value => value.Is(Unit.Instance));
        }
    }
}
