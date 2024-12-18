using System;
using System.IO;
using System.Linq;
using System.Text;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp;

[TestClass]
public class StreamTest
{
    [TestMethod]
    public void ArrayStreamTest()
    {
        // When using an array or `IReadOnlyList<T>` as the source
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var parser = Any<int>().FoldLeft(0, (x, y) => x + y);

        parser.Parse(source).WillSucceed(value => value.Is(55));
    }

    [TestMethod]
    public void ByteStreamTest()
    {
        // When using `System.IO.Stream` as a byte sequence source
        var source = Enumerable.Repeat(new Random(999), 999).Select(random => (byte)random.Next(256)).ToArray();
        using var stream = new MemoryStream(source);
        var parser = Many1(Any<byte>());

        parser.Parse(stream).WillSucceed(value => value.Is(source));
    }

    [TestMethod]
    public void EnumerableStreamTest()
    {
        // When using an `IEnumerable<T>` as the source
        var source = Enumerable.Range(1, 10);
        var parser = Any<int>().FoldRight(0, (x, y) => x + y);

        parser.Parse(source).WillSucceed(value => value.Is(55));
    }

    [TestMethod]
    public void StringStreamTest()
    {
        // When using a string as the source
        var source = "The quick brown fox jumps over the lazy dog";
        var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

        parser.Parse(source).WillSucceed(value => value.Is(source.Split(' ')));
    }

    [TestMethod]
    public void TextStreamTest()
    {
        // When using `System.IO.Stream` as the source
        var source = "The quick brown fox jumps over the lazy dog";
        using var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(source));
        var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

        parser.Parse(stream).WillSucceed(value => value.Is(source.Split(' ')));
    }

    [TestMethod]
    public void TokenizedStreamTest()
    {
        // Allows using the result of repeatedly applying any parser as a source stream.
        // Enables preprocessing like lexical analysis.

        // Parser that matches a string element surrounded by spaces.
        var token = Many1(LetterOrDigit()).Between(Spaces()).AsString();

        var source = "The quick brown fox jumps over the lazy dog";
        using var stream = StringStream.Create(source);
        using var tokenized = ParsecState.Tokenize(stream, token);

        // Parser that matches any token and returns its length.
        var parser = Many(Any<string>().Map(x => x.Length));

        parser.Parse(tokenized).WillSucceed(value => value.Is(source.Split(' ').Select(x => x.Length)));
    }
}
