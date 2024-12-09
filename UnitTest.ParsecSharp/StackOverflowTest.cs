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
            // Simple recursive loop
            const int count = 1_000_000;
            var parser = SkipMany(Any<int>());
            var source = new int[count];

            _ = parser.Parse(source);
        }

        [TestMethod]
        public void ValueTypesStackOverflowTest()
        {
            // When tokens are value types
            const int count = 100_000;
            var parser = Many(Any<(int, int, int)>());
            var source = Enumerable.Range(0, count).Select(x => (x, x, x));

            parser.Parse(source).WillSucceed(value => value.Count.Is(count));
        }

        [TestMethod]
        public void ReferenceTypesStackOverflowTest()
        {
            // When tokens are reference types
            const int count = 100_000;
            var parser = Many(Any<Tuple<int, int, int>>());
            var source = Enumerable.Range(0, count).Select(x => Tuple.Create(x, x, x));

            parser.Parse(source).WillSucceed(value => value.Count.Is(count));
        }

        [TestMethod]
        public void RecursiveDataStructuresStackOverflowTest()
        {
            // When traversing extremely deep structures
            const int depth = 10_000;
            var source = Enumerable.Repeat('[', depth).Concat(Enumerable.Repeat(']', depth)).ToArray();
            var parser = Fix<int>(self => self.Or(Pure(1234)).Between(Char('['), Char(']'))).End();

            parser.Parse(source).WillSucceed(value => value.Is(1234));
        }
    }
}
#endif
