﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpTest
{
    [TestClass]
    public class ParserTest
    {
        private readonly string _abcdEFGH = "abcdEFGH";

        private readonly string _123456 = "123456";

        [TestMethod]
        public void AnyTest()
        {
            // 全ての値にマッチするパーサを作成します。
            // このパーサは入力が終端の場合にのみ失敗します。

            var parser = Any();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));
        }

        [TestMethod]
        public void EndOfInputTest()
        {
            // 入力の終端にマッチするパーサを作成します。

            var parser = EndOfInput();

            var source = string.Empty;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Unit.Instance));
        }

        [TestMethod]
        public void OneOfTest()
        {
            // 指定したシーケンスに値が含まれている場合に成功するパーサを作成します。

            var parser = OneOf("6789abcde");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void NoneOfTest()
        {
            // 指定したシーケンスに値が含まれていない場合に成功するパーサを作成します。

            var parser = NoneOf("dcba987");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"a\""),
                success => Assert.Fail());

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('1'));
        }

        [TestMethod]
        public void ReturnTest()
        {
            // 成功したという結果を返すパーサを作成します。
            // パーサを連結する際に任意の値を投入したい場合に使用します。

            var parser = Return("success!");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("success!"));
        }

        [TestMethod]
        public void FailTest()
        {
            // 失敗したという結果を返すパーサを作成します。

            var parser = Fail<Unit>();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"a\""),
                success => Assert.Fail());
        }

        [TestMethod]
        public void FailTest1()
        {
            // 失敗したという結果を返すパーサを作成します。
            // エラーメッセージを記述することができます。

            var parser = Fail<Unit>(_ => "errormessagetest");

            var source = _abcdEFGH;
            var result = parser.Parse(source);
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): errormessagetest"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void ChoiceTest()
        {
            // parsers を前から1つずつ適用し、最初に成功したものを結果として返すパーサを作成します。
            // 全て失敗した場合、最後の失敗を全体の失敗として返します。

            // 'c'、または 'b'、または 'a' のどれかにマッチするパーサ。
            var parser = Choice(Char('c'), Char('b'), Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void SequenceTest()
        {
            // parsers に順にマッチングし、その結果を連結したシーケンスを返すパーサを作成します。

            // 'a', 'b', 'c', 'd' にマッチし、['a', 'b', 'c', 'd'] を "abcd" に変換して返すパーサ。
            var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).ToStr();

            var source = _abcdEFGH;

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcd"));

            var source2 = "abCDEF";
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 3): Unexpected \"C\""),
                success => Assert.Fail());
        }

        [TestMethod]
        public void TryTest()
        {
            // parser によるパースを実行し、それが失敗した場合は resume の評価を行い結果として返すパーサを作成します。
            // パース失敗時は入力は消費されません。

            // 'a' にマッチし、成功した場合は 'a'、失敗した場合は 'x' を返すパーサ。
            var parser = Try(Char('a'), () => 'x');

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('x'));
        }

        [TestMethod]
        public void OptionalTest()
        {
            // parser によるパースを実行し、その結果を破棄し、常に成功するパーサを作成します。

            // Digitへのマッチングを行い、その値を破棄し、Anyにマッチするパーサ。
            var parser = Optional(Digit()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
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
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { 'a', 'a' }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected \"1\""),
                success => Assert.Fail());
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
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { 'a', 'b', 'c', 'd' }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Enumerable.Empty<char>()));
        }

        [TestMethod]
        public void Many1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // 1回もマッチしなかった場合、パーサは失敗を返します。
            var parser = Many1(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { 'a', 'b', 'c', 'd' }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
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
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { 'a', 'b', 'c', 'd', 'E' }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void SkipManyTest()
        {
            // parser に0回以上繰り返しマッチし、その結果を破棄するパーサを作成します。

            // Lower に0回以上繰り返しマッチし、その結果を破棄し、Anyにマッチした結果を返すパーサ。
            var parser = SkipMany(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('1'));
        }

        [TestMethod]
        public void SkipMany1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果を破棄するパーサを作成します。

            // Lower に1回以上繰り返しマッチし、その結果を破棄し、Anyにマッチした結果を返すパーサ。
            var parser = SkipMany1(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        private readonly string _commanum = "123,456,789";

        [TestMethod]
        public void SepByTest()
        {
            // separator によって区切られた形の parser が0回以上繰り返す入力にマッチするパーサを作成します。
            // separator が返す結果は破棄されます。

            // [ 1*Number *( "," 1*Number ) ]
            var parser = Many1(Number()).ToStr().SepBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                 fail => Assert.Fail(),
                 success => success.Value.Is(new[] { "123456" }));

            var source3 = _abcdEFGH;
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Enumerable.Empty<string>()));
        }

        [TestMethod]
        public void SepBy1Test()
        {
            // separatorによって区切られた形の parser が1回以上繰り返す入力にマッチするパーサを作成します。

            // 1*Number *( "," 1*Number )
            var parser = Many1(Number()).ToStr().SepBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                 fail => Assert.Fail(),
                 success => success.Value.Is(new[] { "123456" }));

            var source3 = _abcdEFGH;
            parser.Parse(source3).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void EndByTest()
        {
            // 末尾に separator が付いた形の parser が0回以上繰り返すものにマッチするパーサを作成します。

            // *( 1*Number "," )
            var parser = Many1(Number()).ToStr().EndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Enumerable.Empty<string>()));
        }

        [TestMethod]
        public void EndBy1Test()
        {
            // 末尾に separator が付いた形の parser が1回以上繰り返すものにマッチするパーサを作成します。

            // 1*( 1*Number "," )
            var parser = Many1(Number()).ToStr().EndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void SepEndByTest()
        {
            // SepBy、または Endby のどちらかとして振る舞うパーサを作成します。

            // [ 1*Number *( "," 1*Number ) [ "," ] ]
            var parser = Many1(Number()).ToStr().SepEndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => success.Value.Is(new[] { "123456" }));

            var source3 = _commanum + ",";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));
        }

        [TestMethod]
        public void SepEndBy1Test()
        {
            // SepBy1、または Endby1 のどちらかとして振る舞うパーサを作成します。

            // 1*Number *( "," 1*Number ) [ "," ]
            var parser = Many1(Number()).ToStr().SepEndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => success.Value.Is(new[] { "123456" }));

            var source3 = _commanum + ",";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "123", "456", "789" }));
        }

        [TestMethod]
        public void ChainLTest()
        {
            // 1個以上の値と演算子に交互にマッチし、指定した演算を左から順に適用するパーサを作成します。

            // '+'、または'-'にマッチし、それぞれ(x + y)、(x - y)の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').FMap(_ => new Func<int, int, int>((x, y) => x + y)),
                Char('-').FMap(_ => new Func<int, int, int>((x, y) => x - y)));

            // 1文字以上の数字にマッチし、intに変換するパーサ。
            var num = Many1(Digit()).ToStr().FMap(x => int.Parse(x));

            // num *(op num)
            var parser = num.ChainL(op);

            var source = "10+5-3+1";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(((10 + 5) - 3) + 1));

            var source2 = "100-20-5+50";
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(((100 - 20) - 5) + 50));

            var source3 = "123";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void ChainRTest()
        {
            // 1個以上の値と演算子に交互にマッチし、指定した演算を右から順に適用するパーサを作成します

            // '+'、または'-'にマッチし、それぞれ(x + y)、(x - y)の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').FMap(_ => new Func<int, int, int>((x, y) => x + y)),
                Char('-').FMap(_ => new Func<int, int, int>((x, y) => x - y)));

            // 1文字以上の数字にマッチし、intに変換するパーサ。
            var num = Many1(Digit()).ToStr().FMap(x => int.Parse(x));

            // num *( op num )
            var parser = num.ChainR(op);

            var source = "10+5-3+1";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(10 + (5 - (3 + 1))));

            var source2 = "100-20-5+50";
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(100 - (20 - (5 + 50))));

            var source3 = "123";
            parser.Parse(source3).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void FoldLTest()
        {
            // 初期値とアキュムレータを引数にとり、パースした結果を右から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して左から (x => accum - x) を繰り返し適用するパーサ。
            var parser = Digit().ToStr().FMap(int.Parse).FoldL(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(((((10 - 1) - 2) - 3) - 4) - 5));
        }

        [TestMethod]
        public void FoldRTest()
        {
            // 初期値とアキュムレータを引数にとり、パース結果を右から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して右から (x => x - accum) を繰り返し適用するパーサ。
            var parser = Digit().ToStr().FMap(int.Parse).FoldR(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
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
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "abc", "dEF" }));
        }

        [TestMethod]
        public void LeftTest()
        {
            // 二つのパーサに順にマッチングし、right の結果を破棄して left の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'a'を返すパーサ。
            var parser = Char('a').Left(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));

            var source2 = "a";
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void RightTest()
        {
            // 二つのパーサに順にマッチングし、left の結果を破棄して right の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'b'を返すパーサ。
            var parser = Char('a').Right(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('b'));

            var source2 = "b";
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void BetweenTest()
        {
            // parser を open と close で挟み込んだ形の規則にマッチするパーサを作成します。
            // open と close の結果は破棄され、中央の結果のみを返します。

            // 1文字以上のLetterを"[]"で包んだものにマッチするパーサ。
            // ( "[" 1*Letter "]" )
            var parser = Many1(Letter()).Between(Char('['), Char(']'));

            var source = "[" + _abcdEFGH + "]";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(_abcdEFGH));

            // Many(Any()) などを parser に渡した場合、終端まで Any にマッチするため、 close は EndOfInput にマッチします。
            var parser2 = Many(Any()).Between(Char('\"'), Char('\"')); // ( dquote *Any dquote ) のつもり
            parser2.Parse("\"abCD1234\"").CaseOf(
                fail => { /* Many(Any()) が abCD1234\" までマッチしてしまうため、close の \" がマッチせずFailになる */ },
                success => Assert.Fail());
            // この形にマッチするパーサを作成するときは、ManyTill を使用してください。
        }

        [TestMethod]
        public void AppendTest()
        {
            // 2つのパーサに順にマッチングし、その結果を結合したシーケンスを返します。

            var source = _abcdEFGH;

            // 1文字 + 1文字
            var parser = Any().Append(Any()).ToStr();
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("ab"));

            // 小文字*n + 1文字
            var parser2 = Many(Lower()).Append(Any()).ToStr();
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcdE"));

            // 1文字 + 小文字*n
            var parser3 = Any().Append(Many(Lower())).ToStr();
            parser3.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcd"));

            // 小文字*n + 大文字*n
            var parser4 = Many(Lower()).Append(Many(Upper())).ToStr();
            parser4.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcdEFGH"));
        }

        [TestMethod]
        public void IgnoreTest()
        {
            // パースした結果を破棄します。
            // 型合わせや、値を捨てることを明示したい場合に使用します。

            var parser = Many1(Lower()).Ignore();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Unit.Instance));

            parser.Right(Any()).Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void MessageTest()
        {
            // パース失敗時のエラーメッセージを書き換えます。

            var parser = Many1(Digit()).Message(state => $"MessageTest Current: '{ state.Current }'");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): MessageTest Current: 'a'"),
                success => Assert.Fail());

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { '1', '2', '3', '4', '5', '6' }));
        }

        [TestMethod]
        public void ErrorTest()
        {
            // パース失敗時にパース処理をAbortします。

            var parser = Many(Lower().Error(state => $"Fatal Error! '{ state }' is not lower char!"));

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 5): Fatal Error! 'E' is not lower char!"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void DoTest()
        {
            // パース実行時に定義したアクションを実行します。
            // Success時、またはSuccess時とFail時にそれぞれ指定したアクションを動作させることができます。

            var source = _abcdEFGH;

            // Lowerのパース成功時にcountの値を1増やします。
            var count = 0;
            var parser = Many(Lower().Do(_ => count++));

            parser.Parse(source);
            count.Is(4);
            parser.Parse(source);
            count.Is(8);

            // Lowerのパース成功時にsuccessの値を1増やし、パース失敗時にfailの値を1増やします。
            // Anyを接続しているので、sourceを最後までパースします。
            var success = 0;
            var fail = 0;
            var parser2 = Many(Lower().Do(_ => success++, _ => fail++).Or(Any()));

            parser2.Parse(source);
            success.Is(4);
            fail.Is(5);
        }
    }
}