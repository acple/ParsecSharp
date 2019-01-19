using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class TextTest
    {
        const string _abcdEFGH = "abcdEFGH";

        const string _123456 = "123456";

        [TestMethod]
        public void CharTest()
        {
            // 指定した一文字にマッチするパーサを作成します。

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
    }
}
