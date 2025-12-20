using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParsecSharp.Bytes;
using static ParsecSharp.Parser;

namespace UnitTest.ParsecSharp.ParserTests.Bytes;

public class BinaryPrimitivesTests
{
    private static readonly IEnumerable<byte> _source = Enumerable.Range(byte.MaxValue / 2 + 1, 9).Select(x => (byte)x);

    [Test]
    public async Task Int16Test()
    {
        var source = _source.ToArray();
        var parser = Int16();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt16(source)));
    }

    [Test]
    public async Task Int16BigEndianTest()
    {
        var source = _source;
        var parser = Int16BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt16(source.Take(2).Reverse().ToArray())));
    }

    [Test]
    public async Task Int32Test()
    {
        var source = _source.ToArray();
        var parser = Int32();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt32(source)));
    }

    [Test]
    public async Task Int32BigEndianTest()
    {
        var source = _source;
        var parser = Int32BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt32(source.Take(4).Reverse().ToArray())));
    }

    [Test]
    public async Task Int64Test()
    {
        var source = _source.ToArray();
        var parser = Int64();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt64(source)));
    }

    [Test]
    public async Task Int64BigEndianTest()
    {
        var source = _source;
        var parser = Int64BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt64(source.Take(8).Reverse().ToArray())));
    }

    [Test]
    public async Task UInt16Test()
    {
        var source = _source.ToArray();
        var parser = UInt16();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt16(source)));
    }

    [Test]
    public async Task UInt16BigEndianTest()
    {
        var source = _source;
        var parser = UInt16BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt16(source.Take(2).Reverse().ToArray())));
    }

    [Test]
    public async Task UInt32Test()
    {
        var source = _source.ToArray();
        var parser = UInt32();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt32(source)));
    }

    [Test]
    public async Task UInt32BigEndianTest()
    {
        var source = _source;
        var parser = UInt32BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt32(source.Take(4).Reverse().ToArray())));
    }

    [Test]
    public async Task UInt64Test()
    {
        var source = _source.ToArray();
        var parser = UInt64();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt64(source)));
    }

    [Test]
    public async Task UInt64BigEndianTest()
    {
        var source = _source;
        var parser = UInt64BigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt64(source.Take(8).Reverse().ToArray())));
    }

    [Test]
    public async Task BooleanTest()
    {
        var parser = Boolean();
        await parser.Parse([(byte)0]).WillSucceed(async value => await Assert.That(value).IsFalse());
        await parser.Parse([(byte)1]).WillSucceed(async value => await Assert.That(value).IsTrue());
    }

    [Test]
    public async Task CharTest()
    {
        var source = _source.ToArray();
        var parser = Char();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToChar(source)));
    }

    [Test]
    public async Task CharBigEndianTest()
    {
        var source = _source;
        var parser = CharBigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToChar(source.Take(2).Reverse().ToArray())));
    }

    [Test]
    public async Task DoubleTest()
    {
        var source = _source.ToArray();
        var parser = Double();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToDouble(source)));
    }

    [Test]
    public async Task DoubleBigEndianTest()
    {
        var source = _source;
        var parser = DoubleBigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToDouble(source.Take(8).Reverse().ToArray())));
    }

    [Test]
    public async Task SingleTest()
    {
        var source = _source.ToArray();
        var parser = Single();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToSingle(source)));
    }

    [Test]
    public async Task SingleBigEndianTest()
    {
        var source = _source;
        var parser = SingleBigEndian();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToSingle(source.Take(4).Reverse().ToArray())));
    }

    [Test]
    public async Task SByteTest()
    {
        var source = _source;
        var parser = SByte();
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo((sbyte)source.First()));
    }

    [Test]
    public async Task Utf8StringTest()
    {
        var source = "English_日本語_中文_한국어_русский язык"u8.ToArray();

        var parser = Utf8String("English".Length);
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("English"));

        var parser2 = parser.Right(Token((byte)'_')).Right(Utf8String("日本語"u8.Length));
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("日本語"));

        var parser3 = Utf8String(source.Length);
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(Encoding.UTF8.GetString(source)));
    }
}
