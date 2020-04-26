using System;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // 逆ポーランド記法の式をパーサで計算してみるネタ
    public static class ReversePolishCalculator
    {
        // 整数または小数にマッチし、double にして返す
        private static readonly Parser<char, double> Num =
            Optional(Char('-') | Char('+'), '+')
                .Append(Many1(DecDigit()))
                .Append(Optional(Char('.').Append(Many1(DecDigit())), Enumerable.Empty<char>()))
                .ToDouble();

        // 四則演算子にマッチし、二項演算関数にマップ
        private static readonly Parser<char, Func<double, double, double>> Op =
            Choice(
                Char('+').MapConst((Func<double, double, double>)((x, y) => x + y)),
                Char('-').MapConst((Func<double, double, double>)((x, y) => x - y)),
                Char('*').MapConst((Func<double, double, double>)((x, y) => x * y)),
                Char('/').MapConst((Func<double, double, double>)((x, y) => x / y)));

        // 各要素間のデリミタ、今回は一文字空白にハードコード
        private static readonly Parser<char, Unit> Delimiter =
            WhiteSpace().Ignore();

        // 式を表す再帰実行パーサ
        // 左再帰の定義: expr = expr expr op / num
        // 左再帰の除去: expr = num *( expr op )
        private static readonly Parser<char, double> Expr =
            Num.Chain(x => from _ in Delimiter
                           from y in Expr
                           from __ in Delimiter
                           from func in Op
                           select func(x, y));

        // パーサの実行
        public static Result<char, double> Parse(string source)
            => Expr.End().Parse(source);
    }
}
