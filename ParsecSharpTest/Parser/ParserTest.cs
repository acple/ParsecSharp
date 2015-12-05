using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var parser = Any();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('a'));
        }

        [TestMethod]
        public void EndOfInputTest()
        {
            var parser = EndOfInput();

            var source = string.Empty;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(Unit.Instance));
        }

        [TestMethod]
        public void OneOfTest()
        {
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
            var parser = NoneOf("dcba987");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected token 'a' -- operator: Satisfy"),
                success => Assert.Fail());

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is('1'));
        }

        [TestMethod]
        public void ErrorTest()
        {
            var parser = Error();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected token 'a' -- operator: Error"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void ErrorTest1()
        {
            var parser = Error("errormessagetest");

            var source = _abcdEFGH;
            var result = parser.Parse(source);
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): errormessagetest"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void ChoiceTest()
        {
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
            var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).ToStr();

            var source = _abcdEFGH;

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcd"));

            var source2 = "abCDEF";
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 3): Unexpected token 'C' -- operator: Satisfy"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void TryTest()
        {
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
            var parser = LookAhead(Letter()).Append(Any());

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { 'a', 'a' }));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): Unexpected token '1' -- operator: Satisfy"),
                success => Assert.Fail());
        }

        [TestMethod]
        public void ManyTest()
        {
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
            var parser = ManyTill(Any(), Upper());

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
        public void SkipManyTest()
        {
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
            var op = Choice(
                Char('+').FMap(_ => new Func<int, int, int>((x, y) => x + y)),
                Char('-').FMap(_ => new Func<int, int, int>((x, y) => x - y)));
            var num = Many1(Digit()).ToStr().FMap(x => int.Parse(x));
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
            var op = Choice(
                Char('+').FMap(_ => new Func<int, int, int>((x, y) => x + y)),
                Char('-').FMap(_ => new Func<int, int, int>((x, y) => x - y)));
            var num = Many1(Digit()).ToStr().FMap(x => int.Parse(x));
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
            var parser = Digit().ToStr().FMap(int.Parse).FoldL(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(((((10 - 1) - 2) - 3) - 4) - 5));
        }

        [TestMethod]
        public void FoldRTest()
        {
            var parser = Digit().ToStr().FMap(int.Parse).FoldR(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(1 - (2 - (3 - (4 - (5 - 10))))));
        }

        [TestMethod]
        public void RepeatTest()
        {
            var parser = Any().Repeat(3).ToStr().Repeat(2);

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { "abc", "dEF" }));
        }

        [TestMethod]
        public void LeftTest()
        {
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
            var parser = Many1(Letter()).Between(Char('['), Char(']'));

            var source = "[" + _abcdEFGH + "]";
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(_abcdEFGH));
        }

        [TestMethod]
        public void AppendTest()
        {
            var source = _abcdEFGH;

            var parser = Any().Append(Any()).ToStr();
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("ab"));

            var parser2 = Many(Lower()).Append(Any()).ToStr();
            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcdE"));

            var parser3 = Any().Append(Many(Lower())).ToStr();
            parser3.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcd"));

            var parser4 = Many(Lower()).Append(Many(Upper())).ToStr();
            parser4.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("abcdEFGH"));
        }

        [TestMethod]
        public void AndTest()
        {
            var parser = Any().Append(Lower()).ToStr();

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is("ab"));

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => { },
                success => Assert.Fail());
        }

        [TestMethod]
        public void IgnoreTest()
        {
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
        public void OnErrorTest()
        {
            var parser = Many1(Digit()).OnError(state => $"OnErrorTest Current: '{ state.Current }'");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => fail.ToString().Is("Parser Fail (Line: 1, Column: 1): OnErrorTest Current: 'a'"),
                success => Assert.Fail());

            var source2 = _123456;
            parser.Parse(source2).CaseOf(
                fail => Assert.Fail(),
                success => success.Value.Is(new[] { '1', '2', '3', '4', '5', '6' }));
        }
    }
}
