#if !DEBUG
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parsec.Internal;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpTest
{
    [TestClass]
    public class StackOverflowTest
    {
        [TestMethod]
        public void StackOverflowTest1()
        {
            var parser = SkipMany(Any());
            var source = new string('A', 1000000);

            parser.Parse(source);
        }

        [TestMethod]
        public void StackOverflowTest2()
        {
            var parser = Many(Any<(int, int, int)>());
            var source = Enumerable.Range(0, 1000000).Select(x => (x, x, x));

            parser.Parse(source);
        }

        [TestMethod]
        public void StackOverflowTest3()
        {
            var parser = Many(Any<Tuple<int, int, int>>());
            var source = Enumerable.Range(0, 1000000).Select(x => Tuple.Create(x, x, x));

            parser.Parse(source);
        }
    }
}
#endif
