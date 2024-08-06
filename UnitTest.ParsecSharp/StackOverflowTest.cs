#if !DEBUG
using System;
using System.Linq;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public class StackOverflowTest
    {
        [TestMethod]
        public void SimpleRecursionStackOverflowTest()
        {
            // 単純な再帰ループ
            const int count = 1_000_000;
            var parser = SkipMany(Any<int>());
            var source = new int[count];

            _ = parser.Parse(source);
        }

        [TestMethod]
        public void ValueTypesStackOverflowTest()
        {
            // トークンが値型の場合
            const int count = 100_000;
            var parser = Many(Any<(int, int, int)>());
            var source = Enumerable.Range(0, count).Select(x => (x, x, x));

            parser.Parse(source).WillSucceed(value => value.Count().Is(count));
        }

        [TestMethod]
        public void ReferenceTypesStackOverflowTest()
        {
            // トークンが参照型の場合
            const int count = 100_000;
            var parser = Many(Any<Tuple<int, int, int>>());
            var source = Enumerable.Range(0, count).Select(x => Tuple.Create(x, x, x));

            parser.Parse(source).WillSucceed(value => value.Count().Is(count));
        }

        [TestMethod]
        public void RecursiveDataStructuresStackOverflowTest()
        {
            // 極端に深い構造を辿る場合
            const int depth = 10_000;
            var source = Enumerable.Repeat('[', depth).Concat(Enumerable.Repeat(']', depth)).ToArray();
            var parser = Fix<int>(self => self.Or(Pure(1234)).Between(Char('['), Char(']'))).End();

            parser.Parse(source).WillSucceed(value => value.Is(1234));
        }
    }
}
#endif
