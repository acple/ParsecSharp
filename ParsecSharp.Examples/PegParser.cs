using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // PEG parser implementation with capturing extension
    public class PegParser
    {
        public static Parser<char, IReadOnlyDictionary<string, Parser<char, string[]>>> Parser { get; } = CreateParser();

        private static Parser<char, IReadOnlyDictionary<string, Parser<char, string[]>>> CreateParser()
        {
            var whitespace = OneOf(" \t").Or(EndOfLine()).Ignore();
            var comment = Char('#').Right(Match(EndOfLine().Ignore() | EndOfInput()));
            var spacing = SkipMany(whitespace | comment);

            var arrow = String("<-").Right(spacing);

            var slash = Char('/').Right(spacing);

            var dot = Char('.').Right(spacing);

            var question = Char('?').Right(spacing);
            var asterisk = Char('*').Right(spacing);
            var plus = Char('+').Right(spacing);

            var and = Char('&').Right(spacing);
            var not = Char('!').Right(spacing);

            var open = Char('(').Right(spacing);
            var close = Char(')').Right(spacing);
            var captureOpen = Char('{').Right(spacing);
            var captureClose = Char('}').Right(spacing);

            var word = AsciiLetter();
            var digit = DecDigit();
            var underscore = Char('_');

            var identifier = (word | underscore).Append(Many(word | digit | underscore)).AsString().Left(spacing);

            var escapedChar = Char('\\').Right(Choice(
                Char('n').Map(_ => '\n'),
                Char('r').Map(_ => '\r'),
                Char('t').Map(_ => '\t'),
                Char('\''),
                Char('"'),
                Char('['),
                Char(']'),
                Char('\\'),
                Char('u').Right(HexDigit().Repeat(4).AsString())
                    .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)), // JSON-style unicode escape
                Any()));

            var character = escapedChar | Any();

            var range =
                from start in character
                from _ in Char('-')
                from end in character
                select Satisfy(x => start <= x && x <= end);

            var literal = (character.Quote(Char('\'')).AsString() | character.Quote(Char('"')).AsString()).Left(spacing);

            var charsetElement = range | character.Map(Char);
            var charsetExcept = charsetElement.Quote(String("[^"), Char(']')).Map(Any().Except);
            var charset = charsetElement.Quote(Char('['), Char(']')).Map(Choice);

            var expression = Fix<Rule>(expression =>
            {
                var primary = Choice(
                    dot.MapConst(new Rule(Any().AsString().Map(value => new Result(value)))),
                    charsetExcept.Map(parser => new Rule(parser.AsString().Map(value => new Result(value)))),
                    charset.Map(parser => new Rule(parser.AsString().Map(value => new Result(value)))),
                    literal.Map(value => new Rule(String(value).Map(value => new Result(value)))),
                    identifier.Left(Not(arrow)).Map(name => new Rule(dict => Delay(() => dict[name].Resolve(dict)))),
                    expression.Between(open, close),
                    expression.Between(captureOpen, captureClose).Map(rule => new Rule(dict => rule.Resolve(dict).Map(result => result.Capture()))));

                var suffixed =
                    primary.Bind(rule => Choice(
                        question.MapConst(new Rule(dict => Optional(rule.Resolve(dict), Result.Empty))),
                        asterisk.MapConst(new Rule(dict => Many(rule.Resolve(dict)).Map(Result.Concat))),
                        plus.MapConst(new Rule(dict => Many1(rule.Resolve(dict)).Map(Result.Concat))),
                        spacing.MapConst(rule)));

                var prefixed = Choice(
                    and.Right(suffixed).Map(rule => new Rule(dict => LookAhead(rule.Resolve(dict)))),
                    not.Right(suffixed).Map(rule => new Rule(dict => Not(rule.Resolve(dict), Result.Empty))),
                    suffixed);

                var seq = Many1(prefixed)
                    .Map(rules => new Rule(dict => Sequence(rules.Select(rule => rule.Resolve(dict))).Map(Result.Concat)));

                return seq.SeparatedBy1(slash)
                    .Map(rules => new Rule(dict => Choice(rules.Select(rule => rule.Resolve(dict)))))
                    .AbortIfEntered();
            });

            var definition =
                from name in identifier
                from _ in arrow
                from rule in expression
                select (name, rule);

            return spacing.Right(Many1(definition)).End().Map(definitions =>
            {
                var dict = definitions.ToDictionary(x => x.name, x => x.rule);
                var parsers = definitions
                    .Where(x => !x.name.StartsWith("_"))
                    .ToDictionary(x => x.name, x => x.rule.Resolve(dict).Map(result => result.Matches.Prepend(result.Value).ToArray()));
                return parsers as IReadOnlyDictionary<string, Parser<char, string[]>>;
            });
        }

        public Result<char, IReadOnlyDictionary<string, Parser<char, string[]>>> Parse(string peg)
            => Parser.Parse(peg);
    }

    // recursive type alias: Rule = IReadOnlyDictionary<string, Rule> -> Parser<char, Result>
    file readonly struct Rule(Func<IReadOnlyDictionary<string, Rule>, Parser<char, Result>> resolver)
    {
        public Rule(Parser<char, Result> parser) : this(_ => parser)
        { }

        public Parser<char, Result> Resolve(IReadOnlyDictionary<string, Rule> dict)
            => resolver(dict);
    }

    file class Result(string value, IEnumerable<string> matches)
    {
        public static Result Empty { get; } = new(string.Empty);

        public string Value { get; } = value;

        public IEnumerable<string> Matches { get; } = matches;

        public Result(string value) : this(value, [])
        { }

        public Result Capture()
            => new(this.Value, this.Matches.Prepend(this.Value));

        public static Result Concat(IEnumerable<Result> results)
            => results.Aggregate(Empty, (left, right) => new(left.Value + right.Value, ConcatCaptures(left.Matches, right.Matches)));

        private static IEnumerable<string> ConcatCaptures(IEnumerable<string> left, IEnumerable<string> right)
            => Empty.Matches is var empty && left == empty ? right : right == empty ? left : left.Concat(right);
    }
}
