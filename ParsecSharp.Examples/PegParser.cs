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
        public interface IMatchResult
        {
            string Match { get; }

            IReadOnlyList<IMatchResult> Captures { get; }

            IReadOnlyList<string> AllMatches { get; }
        }

        public static IParser<char, IReadOnlyDictionary<string, IParser<char, IMatchResult>>> Parser { get; } = CreateParser();

        private static IParser<char, IReadOnlyDictionary<string, IParser<char, IMatchResult>>> CreateParser()
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

            var unescapedChar = Any().Except(Char('\\'));
            var escapedChar = Char('\\').Right(Choice(
                Char('n').Map(_ => '\n'),
                Char('r').Map(_ => '\r'),
                Char('t').Map(_ => '\t'),
                Char('u').Right(HexDigit().Repeat(4).AsString())
                    .Map(hex => (char)int.Parse(hex, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)), // JSON-style unicode escape
                Any().Except(Char('u'))));

            var character = unescapedChar | escapedChar;

            var range =
                from start in character
                from _ in Char('-')
                from end in character
                select Satisfy(x => start <= x && x <= end);

            var literal = (character.Quote(Char('\'')) | character.Quote(Char('"'))).AsString().Left(spacing);

            var charsetElement = range | character.Map(Char);
            var charsetExcept = charsetElement.Quote(String("[^"), Char(']')).Map(Any().Except).Left(spacing);
            var charset = charsetElement.Quote(Char('['), Char(']')).Map(Choice).Left(spacing);

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

                var suffixed = primary
                    .Bind(rule => Choice(
                        question.MapConst(new Rule(dict => Optional(rule.Resolve(dict), Result.Empty))),
                        asterisk.MapConst(new Rule(dict => Many(rule.Resolve(dict)).Map(Result.Concat))),
                        plus.MapConst(new Rule(dict => Many1(rule.Resolve(dict)).Map(Result.Concat))),
                        Pure(rule)));

                var prefixed = Choice(
                    and.Right(suffixed).Map(rule => new Rule(dict => LookAhead(rule.Resolve(dict)))),
                    not.Right(suffixed).Map(rule => new Rule(dict => Not(rule.Resolve(dict), Result.Empty))),
                    suffixed);

                var sequence = Many1(prefixed)
                    .Map(rules => rules.Count == 1 ? rules.First() : new Rule(dict => Sequence(rules.Select(rule => rule.Resolve(dict))).Map(Result.Concat)));

                return sequence.SeparatedBy1(slash)
                    .Map(rules => rules.Count == 1 ? rules.First() : new Rule(dict => Choice(rules.Select(rule => rule.Resolve(dict)))))
                    .AbortIfEntered();
            });

            var definition =
                from name in identifier
                from _ in arrow
                from rule in expression
                select (name, rule);

            var grammar = spacing.Right(Many1(definition)).End()
                .Map(definitions => definitions.ToDictionary(x => x.name, x => x.rule))
                .Map(dict => dict
                    .Where(x => !x.Key.StartsWith("_"))
                    .ToDictionary(x => x.Key, x => x.Value.Resolve(dict) as IParser<char, IMatchResult>));

            return grammar;
        }

        public IResult<char, IReadOnlyDictionary<string, IParser<char, IMatchResult>>> Parse(string peg)
            => Parser.Parse(peg);
    }

    // recursive type alias: Rule = IReadOnlyDictionary<string, Rule> -> IParser<char, Result>
    file readonly struct Rule(Func<IReadOnlyDictionary<string, Rule>, IParser<char, Result>> resolver)
    {
        public Rule(IParser<char, Result> parser) : this(_ => parser)
        { }

        public IParser<char, Result> Resolve(IReadOnlyDictionary<string, Rule> dict)
            => resolver(dict);
    }

    file class Result(string value, IReadOnlyList<Result> captures) : PegParser.IMatchResult
    {
        public static Result Empty { get; } = new(string.Empty);

        private readonly IReadOnlyList<Result> _captures = captures;

        public string Match { get; } = value;

        public IReadOnlyList<PegParser.IMatchResult> Captures => this._captures;

        public IReadOnlyList<string> AllMatches => this.Flatten().ToArray();

        public Result(string value) : this(value, [])
        { }

        public Result Capture()
            => new(this.Match, [this]);

        private IEnumerable<string> Flatten()
            => this._captures.SelectMany(result => result.Flatten()).Prepend(this.Match);

        public static Result Concat(IEnumerable<Result> results)
            => results.Aggregate(Empty, (left, right) => new(left.Match + right.Match, ConcatCaptures(left._captures, right._captures)));

        private static IReadOnlyList<Result> ConcatCaptures(IReadOnlyList<Result> left, IReadOnlyList<Result> right)
            => left.Count == 0 ? right : right.Count == 0 ? left : [.. left, .. right];
    }
}
