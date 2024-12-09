using System;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // Calculate expressions in Reverse Polish Notation using a parser
    public class ReversePolishCalculator
    {
        // Matches integers or decimals and returns them as double
        private static readonly IParser<char, double> Number =
            Optional(OneOf("-+"), '+')
                .Append(Many1(DecDigit()))
                .AppendOptional(Char('.').Append(Many1(DecDigit())))
                .ToDouble();

        // Matches arithmetic operators and maps them to binary functions
        private static readonly IParser<char, Func<double, double, double>> Op =
            Choice(
                Char('+').Map(_ => (Func<double, double, double>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<double, double, double>)((x, y) => x - y)),
                Char('*').Map(_ => (Func<double, double, double>)((x, y) => x * y)),
                Char('/').Map(_ => (Func<double, double, double>)((x, y) => x / y)));

        // Recursive parser for expressions
        // Removing left recursion: expr = num *( expr op )
        // Original left-recursive definition: expr = expr expr op / num
        private static readonly IParser<char, double> Expr =
            Number
                .Chain(x =>
                    from y in Expr
                    from func in Op
                    select func(x, y))
                .Between(Spaces());

        public static IParser<char, double> Parser { get; } = Expr.End();

        public IResult<char, double> Parse(string source)
            => Parser.Parse(source);
    }
}
