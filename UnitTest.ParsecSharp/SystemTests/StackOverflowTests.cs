using System;
using System.Linq;
using System.Threading.Tasks;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.SystemTests;

public class StackOverflowTests
{
    [Test]
    public void SimpleRecursionStackOverflowTest()
    {
        // Simple recursive loop
        const int count = 1_000_000;
        var parser = SkipMany(Any<int>());
        var source = new int[count];

        _ = parser.Parse(source);
    }

    [Test]
    public async Task ValueTypesStackOverflowTest()
    {
        // When tokens are value types
        const int count = 100_000;
        var parser = Many(Any<(int, int, int)>());
        var source = Enumerable.Range(0, count).Select(x => (x, x, x));

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value.Count).IsEqualTo(count));
    }

    [Test]
    public async Task ReferenceTypesStackOverflowTest()
    {
        // When tokens are reference types
        const int count = 100_000;
        var parser = Many(Any<Tuple<int, int, int>>());
        var source = Enumerable.Range(0, count).Select(x => Tuple.Create(x, x, x));

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value.Count).IsEqualTo(count));
    }

    [Test]
    public async Task RecursiveDataStructuresStackOverflowTest()
    {
        // When traversing extremely deep structures
        const int depth = 10_000;
        var source = Enumerable.Repeat('[', depth).Concat(Enumerable.Repeat(']', depth)).ToArray();
        var parser = Fix<int>(self => self.Or(Pure(1234)).Between(Char('['), Char(']'))).End();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(1234));
    }
}
