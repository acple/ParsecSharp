using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParsecSharp.Bytes;
using static ParsecSharp.Parser;

namespace UnitTest.ParsecSharp;

public class BytesTest
{
    private static readonly IEnumerable<byte> _source = Enumerable.Range(byte.MaxValue / 2 + 1, 9).Select(x => (byte)x);

    [Test]
    public async Task SignedTest()
    {
        var source = _source.ToArray();

        var int16 = Int16();
        await int16.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt16(source)));

        var int32 = Int32();
        await int32.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt32(source)));

        var int64 = Int64();
        await int64.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt64(source)));
    }

    [Test]
    public async Task UnsignedTest()
    {
        var source = _source.ToArray();

        var uint16 = UInt16();
        await uint16.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt16(source)));

        var uint32 = UInt32();
        await uint32.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt32(source)));

        var uint64 = UInt64();
        await uint64.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt64(source)));
    }

    [Test]
    public async Task SignedBigEndianTest()
    {
        var int16be = Int16BigEndian();
        await int16be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt16(_source.Take(2).Reverse().ToArray())));

        var int32be = Int32BigEndian();
        await int32be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt32(_source.Take(4).Reverse().ToArray())));

        var int64be = Int64BigEndian();
        await int64be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToInt64(_source.Take(8).Reverse().ToArray())));
    }

    [Test]
    public async Task UnsignedBigEndianTest()
    {
        var uint16be = UInt16BigEndian();
        await uint16be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt16(_source.Take(2).Reverse().ToArray())));

        var uint32be = UInt32BigEndian();
        await uint32be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt32(_source.Take(4).Reverse().ToArray())));

        var uint64be = UInt64BigEndian();
        await uint64be.Parse(_source).WillSucceed(async value => await Assert.That(value).IsEqualTo(BitConverter.ToUInt64(_source.Take(8).Reverse().ToArray())));
    }

    [Test]
    public async Task Utf8StringTest()
    {
        var sourceString = "English_日本語_中文_한국어_русский язык";
        var utf8 = new UTF8Encoding(false);
        var source = utf8.GetBytes(sourceString);

        var parser = Utf8String("English".Length);
        await parser.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("English"));

        var parser2 = parser.Right(Token((byte)'_')).Right(Utf8String(utf8.GetByteCount("日本語")));
        await parser2.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo("日本語"));

        var parser3 = Utf8String(source.Length);
        await parser3.Parse(source).WillSucceed(async value => await Assert.That(value).IsEqualTo(sourceString));
    }
}
