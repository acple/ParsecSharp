using System;
using System.IO;
using System.Linq;
using System.Text;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void ArrayStreamTest()
        {
            // 任意の型の配列、あるいは IReadOnlyList<T> をソースにする場合
            var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var parser = Any<int>().FoldL(0, (x, y) => x + y);

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(55));
        }

        [TestMethod]
        public void ByteStreamTest()
        {
            // System.IO.Stream をバイト列としてソースにする場合
            var source = Enumerable.Repeat(new Random("seed".GetHashCode()), 999).Select(random => (byte)random.Next(256)).ToArray();
            using var stream = new MemoryStream(source);
            var parser = Many1(Any<byte>());

            parser.Parse(stream).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(source));
        }

        [TestMethod]
        public void EnumerableStreamTest()
        {
            // 任意の型の IEnumerable<T> をソースにする場合
            var source = Enumerable.Range(1, 10);
            var parser = Any<int>().FoldR(0, (x, y) => x + y);

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(55));
        }

        [TestMethod]
        public void StringStreamTest()
        {
            // 文字列をソースにする場合
            var source = "The quick brown fox jumps over the lazy dog";
            var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

            parser.Parse(source).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(source.Split(' ')));
        }

        [TestMethod]
        public void TextStreamTest()
        {
            // System.IO.Stream をソースにする場合
            var source = "The quick brown fox jumps over the lazy dog";
            using var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(source));
            var parser = Many(Many1(Letter()).Between(Spaces()).AsString()).ToArray();

            parser.Parse(stream).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(source.Split(' ')));
        }

        [TestMethod]
        public void TokenizedStreamTest()
        {
            // 任意のパーサを繰り返し適用した結果をソースストリームとして利用可能にします。
            // 字句解析等の前段処理を可能にします。

            // 空白に挟まれた文字列を1要素として返すパーサ。
            var token = Many1(LetterOrDigit()).Between(Spaces()).AsString();

            var source = "The quick brown fox jumps over the lazy dog";
            using var stream = new StringStream(source);
            var tokenized = stream.Tokenize(token);

            // 任意のトークンにマッチし、その長さを返すパーサ。
            var parser = Many(Any<string>().Map(x => x.Length));

            parser.Parse(tokenized).CaseOf(
                fail => Assert.Fail(fail.ToString()),
                success => success.Value.Is(source.Split(' ').Select(x => x.Length)));
        }
    }
}
