using System;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // 逆ポーランド記法の式をパーサで計算してみるネタ
    public static class ReversePolishCalculator
    {
        // 整数または小数にマッチし、double にして返す
        private static readonly Parser<char, double> Number =
            Optional(Char('-') | Char('+'), '+')
                .Append(Many1(DecDigit()))
                .Append(Optional(Char('.').Append(Many1(DecDigit())), ".0"))
                .ToDouble();

        // 四則演算子にマッチし、二項演算関数にマップ
        private static readonly Parser<char, Func<double, double, double>> Op =
            Choice(
                Char('+').Map(_ => (Func<double, double, double>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<double, double, double>)((x, y) => x - y)),
                Char('*').Map(_ => (Func<double, double, double>)((x, y) => x * y)),
                Char('/').Map(_ => (Func<double, double, double>)((x, y) => x / y)));

        // 式を表す再帰実行パーサ
        // 左再帰の定義: expr = expr expr op / num
        // 左再帰の除去: expr = num *( expr op )
        private static readonly Parser<char, double> Expr =
            Number
                .Chain(x =>
                    from y in Expr
                    from func in Op
                    select func(x, y))
                .Between(Spaces());

        public static Parser<char, double> Parser { get; } =
            Expr.End();

        public static Result<char, double> Parse(string source)
            => Parser.Parse(source);
    }
}
