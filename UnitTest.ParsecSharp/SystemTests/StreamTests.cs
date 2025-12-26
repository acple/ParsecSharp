using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp.SystemTests;

public class StreamTests
{
    [Test]
    public async Task ArrayStreamTest()
    {
        // When using an array or `IReadOnlyList<T>` as the source
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var parser = Any<int>().FoldLeft(0, (x, y) => x + y);

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(55));
    }

    [Test]
    public async Task ByteStreamTest()
    {
        // When using `System.IO.Stream` as a byte sequence source
        var source = Enumerable.Repeat(new Random(999), 999).Select(random => (byte)random.Next(256)).ToArray();
        using var stream = new MemoryStream(source);
        var parser = Many1(Any<byte>());

        await parser.Parse(stream).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(source));
    }

    [Test]
    public async Task EnumerableStreamTest()
    {
        // When using an `IEnumerable<T>` as the source
        var source = Enumerable.Range(1, 10);
        var parser = Any<int>().FoldRight(0, (x, y) => x + y);

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(55));
    }

    [Test]
    public async Task StringStreamTest()
    {
        // When using a string as the source
        var source = "The quick brown fox jumps over the lazy dog";
        var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(source.Split(' ')));
    }

    [Test]
    public async Task TextStreamTest()
    {
        // When using `System.IO.Stream` as the source
        var source = "The quick brown fox jumps over the lazy dog";
        using var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(source));
        var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

        await parser.Parse(stream).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(source.Split(' ')));
    }

    [Test]
    public async Task TokenizedStreamTest()
    {
        // Allows using the result of repeatedly applying any parser as a source stream.
        // Enables preprocessing like lexical analysis.

        // Parser that matches a string element surrounded by spaces.
        var token = Many1(LetterOrDigit()).Between(Spaces()).AsString();

        var source = "The quick brown fox jumps over the lazy dog";
        using var stream = StringStream.Create(source);
        using var tokenized = stream.Tokenize(token);

        // Parser that matches any token and returns its length.
        var parser = Many(Any<string>().Map(x => x.Length));

        await parser.Parse(tokenized).WillSucceed(async value => await Assert.That(value).IsSequentiallyEqualTo(source.Split(' ').Select(x => x.Length)));
    }
}
