using System;
using System.Linq;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace ParsecSharp.Examples
{
    // 逆ポーランド記法の式をパーサで計算してみるネタ
    // Chainが欲しくなったのはコレを定義してみたかったから(ChainL/Rだとうまいこと定義できなかった)
    public static class ReversePolishCalculator
    {
        // 整数または小数にマッチし、doubleにして返す
        private static readonly Parser<char, double> num =
            Optional(Char('-'), '+')
                .Append(Many1(Digit()))
                .Append(Optional(Char('.').Append(Many1(Digit())), Enumerable.Empty<char>()))
                .ToStr().Map(double.Parse);

        // 四則演算子にマッチし、二項演算関数にマップ
        private static readonly Parser<char, Func<double, double, double>> op =
            Choice(
                Char('+').Map(_ => (Func<double, double, double>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<double, double, double>)((x, y) => x - y)),
                Char('*').Map(_ => (Func<double, double, double>)((x, y) => x * y)),
                Char('/').Map(_ => (Func<double, double, double>)((x, y) => x / y)));

        // 各要素間のデリミタ、今回は一文字空白にハードコード
        private static readonly Parser<char, Unit> delimiter =
            WhiteSpace().Ignore();

        // 式を表す再帰実行パーサ
        // 左再帰の定義: expr = expr expr op / num
        // 左再帰の除去: expr = num *( expr op )
        private static readonly Parser<char, double> expr =
            num.Chain(x => from _ in delimiter
                           from y in expr
                           from __ in delimiter
                           from func in op
                           select func(x, y));

        // パーサの実行
        public static Result<char, double> Parse(string source)
            => expr.End().Parse(source);
    }
}
