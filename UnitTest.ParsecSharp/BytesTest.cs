using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ParsecSharp.Bytes;
using static ParsecSharp.Parser;

namespace UnitTest.ParsecSharp;

[TestClass]
public class BytesTest
{
    private static readonly IEnumerable<byte> _source = Enumerable.Range(byte.MaxValue / 2 + 1, 9).Select(x => (byte)x);

    [TestMethod]
    public void SignedTest()
    {
        var source = _source.ToArray();

        var int16 = Int16();
        int16.Parse(source).WillSucceed(value => value.Is(BitConverter.ToInt16(source)));

        var int32 = Int32();
        int32.Parse(source).WillSucceed(value => value.Is(BitConverter.ToInt32(source)));

        var int64 = Int64();
        int64.Parse(source).WillSucceed(value => value.Is(BitConverter.ToInt64(source)));
    }

    [TestMethod]
    public void UnsignedTest()
    {
        var source = _source.ToArray();

        var uint16 = UInt16();
        uint16.Parse(source).WillSucceed(value => value.Is(BitConverter.ToUInt16(source)));

        var uint32 = UInt32();
        uint32.Parse(source).WillSucceed(value => value.Is(BitConverter.ToUInt32(source)));

        var uint64 = UInt64();
        uint64.Parse(source).WillSucceed(value => value.Is(BitConverter.ToUInt64(source)));
    }

    [TestMethod]
    public void SignedBigEndianTest()
    {
        var int16be = Int16BigEndian();
        int16be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToInt16(_source.Take(2).Reverse().ToArray())));

        var int32be = Int32BigEndian();
        int32be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToInt32(_source.Take(4).Reverse().ToArray())));

        var int64be = Int64BigEndian();
        int64be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToInt64(_source.Take(8).Reverse().ToArray())));
    }

    [TestMethod]
    public void UnsignedBigEndianTest()
    {
        var uint16be = UInt16BigEndian();
        uint16be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToUInt16(_source.Take(2).Reverse().ToArray())));

        var uint32be = UInt32BigEndian();
        uint32be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToUInt32(_source.Take(4).Reverse().ToArray())));

        var uint64be = UInt64BigEndian();
        uint64be.Parse(_source).WillSucceed(value => value.Is(BitConverter.ToUInt64(_source.Take(8).Reverse().ToArray())));
    }

    [TestMethod]
    public void Utf8StringTest()
    {
        var sourceString = "English_日本語_中文_한국어_русский язык";
        var utf8 = new UTF8Encoding(false);
        var source = utf8.GetBytes(sourceString);

        var parser = Utf8String("English".Length);
        parser.Parse(source).WillSucceed(value => value.Is("English"));

        var parser2 = parser.Right(Token((byte)'_')).Right(Utf8String(utf8.GetByteCount("日本語")));
        parser2.Parse(source).WillSucceed(value => value.Is("日本語"));

        var parser3 = Utf8String(source.Length);
        parser3.Parse(source).WillSucceed(value => value.Is(sourceString));
    }
}
