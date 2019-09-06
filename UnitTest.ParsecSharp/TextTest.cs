using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class TextTest
    {
        private const string _abcdEFGH = "abcdEFGH";

        private const string _123456 = "123456";

        [TestMethod]
        public void CharTest()
        {
            // 指定した一文字にマッチするパーサを作成します。
            // 汎用の Token パーサと同様の結果を得られます。char 特化のためパフォーマンス面でやや有利です。

            var parser = Char('a');

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
        public void CharIgnoreCaseTest()
        {
            // 指定した一文字に大文字小文字を区別せずマッチするパーサを作成します。

            var parser = CharIgnoreCase('A');

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('a'));
        }

        [TestMethod]
        public void StringTest()
        {
            // 指定した文字列にマッチするパーサを作成します。

            var parser = String("abc");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abc"));
        }

        [TestMethod]
        public void StringIgnoreCaseTest()
        {
            // 指定した文字列に大文字小文字を区別せずマッチするパーサを作成します。

            var parser = StringIgnoreCase("abcde");

            var source = _abcdEFGH;
            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is("abcdE"));
        }

        [TestMethod]
        public void ToIntTest()
        {
            // 結果の文字列を数値に変換するコンビネータです。

            var source = "1234abcd";

            // [0 - 9] にマッチし int に変換したものを返すパーサ。
            var parser = Many1(DecDigit()).ToInt();

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(1234));

            // 変換対象に数値以外の文字を含ませた場合、変換に失敗する。
            var parser2 = Many1(Any()).ToInt();
            parser2.Parse(source).CaseOf(
                fail => fail.Message.Is("Expected digits but was '1234abcd'"),
                success => Assert.Fail(success.ToString()));
        }

        [TestMethod]
        public void OneOfIgnoreCaseTest()
        {
            var source = "z";

            var parser = OneOfIgnoreCase("XYZ");

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('z'));
        }

        [TestMethod]
        public void NoneOfIgnoreCaseTest()
        {
            var source = "z";

            var parser = NoneOfIgnoreCase("XYZ");

            parser.Parse(source).CaseOf(
                fail => { },
                success => Assert.Fail(success.ToString()));

            var parser2 = NoneOfIgnoreCase("ABCD");

            parser2.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is('z'));
        }
    }
}
