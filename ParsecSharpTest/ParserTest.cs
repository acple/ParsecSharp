using System;
using System.Linq;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpTest
{
    [TestClass]
    public class ParserTest
    {
        const string _abcdEFGH = "abcdEFGH";

        const string _123456 = "123456";

        [TestMethod]
        public void AnyTest()
        {
            // 全ての値にマッチするパーサを作成します。
            // このパーサは入力が終端の場合にのみ失敗します。

            var parser = Any();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));
        }

        [TestMethod]
        public void EndOfInputTest()
        {
            // 入力の終端にマッチするパーサを作成します。

            var parser = EndOfInput();

            var source = string.Empty;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(Unit.Instance));
        }

        [TestMethod]
        public void OneOfTest()
        {
            // 指定したシーケンスに値が含まれている場合に成功するパーサを作成します。

            var parser = OneOf("6789abcde");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));

            // IEnumerable<char>を取るオーバーロード。
            var parser2 = OneOf("6789abcde".AsEnumerable());
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            parser2.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void NoneOfTest()
        {
            // 指定したシーケンスに値が含まれていない場合に成功するパーサを作成します。

            var parser = NoneOf("dcba987");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"a\""),
                success => Assert.Fail(success.ToString()));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('1'));
        }

        [TestMethod]
        public void SkipTest()
        {
            // 数を指定してスキップするパーサを作成します。

            var parser = Skip(3).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('d'));

            var parser2 = Skip(8).Right(EndOfInput());
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => { });

            // 指定数スキップできない場合は失敗します。
            var parser3 = Skip(9);
            parser3.Parse(source).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void PureTest()
        {
            // 成功したという結果を返すパーサを作成します。
            // パーサに任意の値を投入する場合に使用します。

            var parser = Pure("success!");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("success!"));
        }

        [TestMethod]
        public void FailTest()
        {
            // 失敗したという結果を返すパーサを作成します。

            var source = _abcdEFGH;

            var parser = Fail<Unit>();
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"a\""),
                success => Assert.Fail(success.ToString()));

            // エラーメッセージを記述することができるオーバーロード。
            var parser2 = Fail<Unit>(_ => "errormessagetest");
            parser2.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): errormessagetest"),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void GetPositionTest()
        {
            // パース位置の Position を取り出すパーサを作成します。
            // このパーサは入力を消費しません。

            // Anyに3回マッチした後、その時点の Position を返すパーサ。
            var parser = Any().Repeat(3).Right(GetPosition());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Column.Is(4));
        }

        [TestMethod]
        public void ChoiceTest()
        {
            // parsers を前から1つずつ適用し、最初に成功したものを結果として返すパーサを作成します。
            // 全て失敗した場合、最後の失敗を全体の失敗として返します。

            // 'c'、'b'、または 'a' のどれかにマッチするパーサ。
            var parser = Choice(Char('c'), Char('b'), Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void SequenceTest()
        {
            // parsers に順にマッチングし、その結果を連結したシーケンスを返すパーサを作成します。

            // 'a' + 'b' + 'c' + 'd' にマッチし、['a', 'b', 'c', 'd'] を "abcd" に変換して返すパーサ。
            var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).ToStr();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abcd"));

            var source2 = "abCDEF";
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 3): Unexpected \"C\""),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void TryTest()
        {
            // parser によるパースを実行し、それが失敗した場合は resume の値を結果として返すパーサを作成します。
            // パース失敗時は入力は消費されません。

            // 'a' にマッチし、成功した場合は 'a'、失敗した場合は 'x' を返すパーサ。
            var parser = Try(Char('a'), 'x');

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('x'));

            // resume の評価を遅延させるオーバーロード。
            var parser2 = Try(Char('a'), () => 'x');
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));
        }

        [TestMethod]
        public void OptionalTest()
        {
            // parser によるパースを実行し、その結果を破棄し、常に成功するパーサを作成します。

            // Digit へのマッチングを行い、その値を破棄し、Any にマッチするパーサ。
            var parser = Optional(Digit()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('2'));
        }

        [TestMethod]
        public void LookAheadTest()
        {
            // 入力を消費せずに parser によるパースを行うパーサを作成します。

            // 入力を消費せずに Letter にマッチし、その後 Any とマッチしその結果を連結するパーサ。
            var parser = LookAhead(Letter()).Append(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a', 'a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"1\""),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void ManyTest()
        {
            // parser に0回以上繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // 1回もマッチしなかった場合、パーサは空のシーケンスを結果として返します。

            // Lower に0回以上繰り返しマッチするパーサ。
            var parser = Many(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.IsEmpty());
        }

        [TestMethod]
        public void Many1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // 1回もマッチしなかった場合、パーサは失敗を返します。

            // Lower に1回以上繰り返しマッチするパーサ。
            var parser = Many1(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void SkipManyTest()
        {
            // parser に0回以上繰り返しマッチし、その結果を破棄するパーサを作成します。

            // Lower に0回以上繰り返しマッチし、その結果を破棄し、Any にマッチした結果を返すパーサ。
            var parser = SkipMany(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('1'));
        }

        [TestMethod]
        public void SkipMany1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果を破棄するパーサを作成します。

            // Lower に1回以上繰り返しマッチし、その結果を破棄し、Any にマッチした結果を返すパーサ。
            var parser = SkipMany1(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void ManyTillTest()
        {
            // terminator にマッチするまでの間 parser に繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // terminator にマッチした結果は破棄されます。

            // 'F' にマッチするまでの間 Any に繰り返しマッチするパーサ。
            var parser = ManyTill(Any(), Char('F'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a', 'b', 'c', 'd', 'E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void SkipTillTest()
        {
            // terminator にマッチするまで parser に繰り返しマッチし、terminator にマッチした結果を返すパーサを作成します。

            // "cd" にマッチするまでの間 Lower をスキップするパーサ。
            var parser = SkipTill(Lower(), String("cd"));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("cd"));

            // "cd" の前に Upper が存在する場合は失敗する。
            var source2 = "xyzABcdef";
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void MatchTest()
        {
            // parser にマッチするまでスキップし、parser にマッチした結果を返すパーサを作成します。

            var source = _abcdEFGH;
            var source2 = _123456;

            // "FG" にマッチするまでスキップするパーサ。
            var parser = Match(String("FG"));

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("FG"));

            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));

            // ( Lower Upper ) にマッチするまでスキップするパーサ。
            var parser2 = Match(Sequence(Lower(), Upper())).ToStr();

            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("dE"));
        }

        const string _commanum = "123,456,789";

        [TestMethod]
        public void SepByTest()
        {
            // separator によって区切られた形の parser が0回以上繰り返す入力にマッチするパーサを作成します。
            // separator が返す結果は破棄されます。

            // [ 1*Number *( "," 1*Number ) ]
            var parser = Many1(Number()).ToStr().SepBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                 fail => Assert.Fail(fail.ToString()),
                 success => success.Value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.IsEmpty());
        }

        [TestMethod]
        public void SepBy1Test()
        {
            // separator によって区切られた形の parser が1回以上繰り返す入力にマッチするパーサを作成します。

            // 1*Number *( "," 1*Number )
            var parser = Many1(Number()).ToStr().SepBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                 fail => Assert.Fail(fail.ToString()),
                 success => success.Value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void EndByTest()
        {
            // 末尾に separator が付いた形の parser が0回以上繰り返すものにマッチするパーサを作成します。

            // *( 1*Number "," )
            var parser = Many1(Number()).ToStr().EndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.IsEmpty());
        }

        [TestMethod]
        public void EndBy1Test()
        {
            // 末尾に separator が付いた形の parser が1回以上繰り返すものにマッチするパーサを作成します。

            // 1*( 1*Number "," )
            var parser = Many1(Number()).ToStr().EndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void SepEndByTest()
        {
            // SepBy、または Endby のどちらかとして振る舞うパーサを作成します。

            // [ 1*Number *( "," 1*Number ) [ "," ] ]
            var parser = Many1(Number()).ToStr().SepEndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => success.Value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void SepEndBy1Test()
        {
            // SepBy1、または Endby1 のどちらかとして振る舞うパーサを作成します。

            // 1*Number *( "," 1*Number ) [ "," ]
            var parser = Many1(Number()).ToStr().SepEndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => success.Value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void ExceptTest()
        {
            // 指定したパーサに対して除外条件を設定したパーサを作成します。

            var source = _123456;

            // '5' 以外の数字にマッチするパーサ。
            var parser = Digit().Except(Char('5'));
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('1'));

            // '5' 以外の数字に連続でマッチし、文字列に変換したものを返すパーサ。
            var parser2 = Many(parser).ToStr();
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("1234"));
        }

        [TestMethod]
        public void ChainTest()
        {
            // 単一パーサから開始し、その結果を元に次のパーサを作成し、失敗するまで繰り返す再帰パーサを作成します。

            // 任意の一文字に連続でマッチし、マッチした文字とその回数を結果として返すパーサ。
            var parser = Any().Map(x => (x, count: 1))
                .Chain(match => Char(match.x).Map(_ => (match.x, match.count + 1)))
                .Map(match => match.x.ToString() + match.count.ToString());

            var source = "aaaaaaaaa";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("a9"));

            var source2 = "aaabbbbcccccdddddd";
            Many(parser).Join().Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("a3b4c5d6"));

            // 本来、自己を最初に参照するパーサを直接記述することはできない(無限再帰となるため)。
            // 有名な二項演算の左再帰定義。
            // expr = expr op digit / digit

            // この定義を変形して左再帰を除去することが可能。
            // 二項演算の左再帰除去後の定義。
            // expr = digit *( op digit )

            // Chain を使うことで左再帰除去後の定義をそのまま記述することができます。
            var expr = Digit().Chain(x => Char('+').Right(Digit()));

            // 直接左再帰定義の例。こちらは実行すると死にます。
            var num = Many1(Digit()).ToInt();
            Parser<char, int> Expr()
                => (from x in Expr() // ここで無限再帰
                    from func in Char('+')
                    from y in num
                    select x + y)
                    .Or(num);
        }

        [TestMethod]
        public void ChainLTest()
        {
            // ChainLはChainを利用して簡単に作成できます。
            Parser<TToken, T> ChainL<TToken, T>(Parser<TToken, T> token, Parser<TToken, Func<T, T, T>> function)
                => token.Chain(x => from func in function
                                    from y in token
                                    select func(x, y));

            // 1個以上の値と演算子に交互にマッチし、指定した演算を左から順に適用するパーサを作成します。

            // '+'、または '-' にマッチし、それぞれ (x + y)、(x - y) の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

            // 1文字以上の数字にマッチし、intに変換するパーサ。
            var num = Many1(Digit()).ToInt();

            // num *( op num )
            var parser = ChainL(num, op);

            var source = "10+5-3+1";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(((10 + 5) - 3) + 1));

            var source2 = "100-20-5+50";
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(((100 - 20) - 5) + 50));

            var source3 = "123";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void ChainRTest()
        {
            // ChainRはChainを利用して簡単に作成できます。
            Parser<TToken, T> ChainR<TToken, T>(Parser<TToken, T> token, Parser<TToken, Func<T, T, T>> function)
                => token.Chain(x => from func in function
                                    from y in ChainR(token, function)
                                    select func(x, y));

            // 1個以上の値と演算子に交互にマッチし、指定した演算を右から順に適用するパーサを作成します

            // '+'、または '-' にマッチし、それぞれ (x + y)、(x - y) の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

            // 1文字以上の数字にマッチし、intに変換するパーサ。
            var num = Many1(Digit()).ToInt();

            // num *( op num )
            var parser = ChainR(num, op);

            var source = "10+5-3+1";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(10 + (5 - (3 + 1))));

            var source2 = "100-20-5+50";
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(100 - (20 - (5 + 50))));

            var source3 = "123";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void FoldLTest()
        {
            // 初期値と集計関数を引数にとり、パースした結果を左から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して左から (x => accum - x) を繰り返し適用するパーサ。
            var parser = Digit().ToStr().ToInt().FoldL(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(((((10 - 1) - 2) - 3) - 4) - 5));
        }

        [TestMethod]
        public void FoldRTest()
        {
            // 初期値と集計関数を引数にとり、パース結果を右から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して右から (x => x - accum) を繰り返し適用するパーサ。
            var parser = Digit().ToStr().ToInt().FoldR(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(1 - (2 - (3 - (4 - (5 - 10))))));
        }

        [TestMethod]
        public void RepeatTest()
        {
            // parser を count 回繰り返しマッチした結果をシーケンスとして返すパーサを作成します。

            //  2*( 3*Any )
            var parser = Any().Repeat(3).ToStr().Repeat(2);

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abc", "dEF"));
        }

        [TestMethod]
        public void LeftTest()
        {
            // 二つのパーサに順にマッチングし、right の結果を破棄して left の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'a'を返すパーサ。
            var parser = Char('a').Left(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));

            var source2 = "a";
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void RightTest()
        {
            // 二つのパーサに順にマッチングし、left の結果を破棄して right の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'b'を返すパーサ。
            var parser = Char('a').Right(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('b'));

            var source2 = "b";
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void BetweenTest()
        {
            // parser を open と close で挟み込んだ形の規則にマッチするパーサを作成します。
            // open と close の結果は破棄され、中央の結果のみを返します。

            // 1文字以上の Letter を "[]" で包んだものにマッチするパーサ。
            // ( "[" 1*Letter "]" )
            var parser = Many1(Letter()).Between(Char('['), Char(']'));

            var source = "[" + _abcdEFGH + "]";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(_abcdEFGH));

            // Many(Any()) などを parser に渡した場合、終端まで Any にマッチするため、 close は EndOfInput にマッチします。
            var parser2 = Many(Any()).Between(Char('\"'), Char('\"')); // ( dquote *Any dquote ) とはならない
            parser2.Parse("\"abCD1234\"").CaseOf(
                fail => { /* Many(Any()) が abCD1234\" までマッチしてしまうため、close の \" がマッチせずFailになる */ },
                success => Assert.Fail(success.ToString()));
            // この形にマッチするパーサを作成したいときは、ManyTill を使用してください。
        }

        [TestMethod]
        public void AppendTest()
        {
            // 2つのパーサに順にマッチングし、その結果を結合したシーケンスを返します。

            var source = _abcdEFGH;

            // 1文字 + 1文字
            var parser = Any().Append(Any()).ToStr();
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("ab"));

            // 小文字*n + 1文字
            var parser2 = Many(Lower()).Append(Any()).ToStr();
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abcdE"));

            // 1文字 + 小文字*n
            var parser3 = Any().Append(Many(Lower())).ToStr();
            parser3.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abcd"));

            // 小文字*n + 大文字*n
            var parser4 = Many(Lower()).Append(Many(Upper())).ToStr();
            parser4.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abcdEFGH"));
        }

        [TestMethod]
        public void IgnoreTest()
        {
            // パースした結果を破棄します。
            // 型合わせや、値を捨てることを明示したい場合に使用します。

            // 1文字以上の小文字にマッチし、その結果を破棄するパーサ。
            var parser = Many1(Lower()).Ignore();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(Unit.Instance));

            parser.Right(Any()).Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void WithMessageTest()
        {
            // パース失敗時のエラーメッセージを書き換えます。

            var parser = Many1(Digit()).WithMessage(state => $"MessageTest Current: '{state.Current.ToString()}'");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): MessageTest Current: 'a'"),
                success => Assert.Fail(success.ToString()));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('1', '2', '3', '4', '5', '6'));
        }

        [TestMethod]
        public void ErrorTest()
        {
            // パース失敗時にパース処理を中止します。

            var parser = Many(Lower().Error(state => $"Fatal Error! '{state.ToString()}' is not a lower char!")).ToStr()
                .Or(Pure("recovery"));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 5): Fatal Error! 'E' is not a lower char!"),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void DoTest()
        {
            // パース実行時に定義したアクションを実行します。
            // 成功時、または 成功時と失敗時にそれぞれ指定したアクションを動作させることができます。

            var source = _abcdEFGH;

            // Lower のパース成功時に count の値を1増やします。
            var count = 0;
            var parser = Many(Lower().Do(_ => count++));

            parser.Parse(source);
            count.Is(4);
            parser.Parse(source);
            count.Is(8);

            // Lower のパース成功時に success の値を1増やし、パース失敗時に fail の値を1増やします。
            // Any を接続しているので、source を最後までパースします。
            var success = 0;
            var fail = 0;
            var parser2 = Many(Lower().Do(_ => success++, _ => fail++).Or(Any()));

            parser2.Parse(source);
            success.Is(4);
            fail.Is(5);
        }

        [TestMethod]
        public void ExceptionTest()
        {
            // 例外発生時は即座にパース処理を中止し、例外の Name をメッセージに含む Fail を返します。
            // 例外による失敗に対して復旧は行われません。

            // null に対して ToString した結果を返し、失敗した場合に "success" を返すことを試みるパーサ。
            var parser = Pure(null as object).Map(x => x.ToString()).Or(Pure("success"));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is(message => message.Contains("Exception 'NullReferenceException' occurred:")),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void FixTest()
        {
            // ローカル変数上やパーサ定義式内に自己再帰パーサを構築する際のヘルパコンビネータです。
            // C#の仕様上、単体で使用する場合は型情報が不足するため、型引数を与える必要があります。

            // 任意の回数の "{}" に挟まれた一文字にマッチするパーサ。
            var parser = Fix<char, char>(self => self.Or(Any()).Between(Char('{'), Char('}')));

            var source = "{{{{{*}}}}}";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('*'));

            // パラメータを取るオーバーロード。柔軟に再帰パーサを記述できます。
            // 有名な回文パーサ。 S ::= "a" S "a" | "b" S "b" | ""
            var parser2 = Fix<char, Parser<char, Unit>, Unit>((self, rest) =>
                Char('a').Right(self(Char('a').Right(rest))) | Char('b').Right(self(Char('b').Right(rest))) | rest);

            var source2 = "abbaabba";
            parser2(EndOfInput()).Parse(source2).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(Unit.Instance));
        }
    }
}
