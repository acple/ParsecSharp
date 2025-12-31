using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using static ParsecSharp.Parser;

namespace ParsecSharp;

public static class Bytes
{
    #region Parser Extensions

    extension<T>(IParser<byte, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<byte, T> Parse(Stream source)
            => parser.Parse(ByteStream.Create(source));
    }

    #endregion

    #region Binary Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, short> Int16()
        => Take(sizeof(short)).Map(BinaryConvert.ToInt16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, short> Int16BigEndian()
        => Take(sizeof(short)).Map(BinaryConvert.ToInt16BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, int> Int32()
        => Take(sizeof(int)).Map(BinaryConvert.ToInt32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, int> Int32BigEndian()
        => Take(sizeof(int)).Map(BinaryConvert.ToInt32BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, long> Int64()
        => Take(sizeof(long)).Map(BinaryConvert.ToInt64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, long> Int64BigEndian()
        => Take(sizeof(long)).Map(BinaryConvert.ToInt64BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, ushort> UInt16()
        => Take(sizeof(ushort)).Map(BinaryConvert.ToUInt16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, ushort> UInt16BigEndian()
        => Take(sizeof(ushort)).Map(BinaryConvert.ToUInt16BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, uint> UInt32()
        => Take(sizeof(uint)).Map(BinaryConvert.ToUInt32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, uint> UInt32BigEndian()
        => Take(sizeof(uint)).Map(BinaryConvert.ToUInt32BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, ulong> UInt64()
        => Take(sizeof(ulong)).Map(BinaryConvert.ToUInt64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, ulong> UInt64BigEndian()
        => Take(sizeof(ulong)).Map(BinaryConvert.ToUInt64BigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, bool> Boolean()
        => Any().Map(BinaryConvert.ToBoolean);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, char> Char()
        => Take(sizeof(char)).Map(BinaryConvert.ToChar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, char> CharBigEndian()
        => Take(sizeof(char)).Map(BinaryConvert.ToCharBigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, float> Single()
        => Take(sizeof(float)).Map(BinaryConvert.ToSingle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, float> SingleBigEndian()
        => Take(sizeof(float)).Map(BinaryConvert.ToSingleBigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, double> Double()
        => Take(sizeof(double)).Map(BinaryConvert.ToDouble);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, double> DoubleBigEndian()
        => Take(sizeof(double)).Map(BinaryConvert.ToDoubleBigEndian);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, sbyte> SByte()
        => Any().Map(x => (sbyte)x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, string> Utf8String(int count)
        => Take(count).MapWithExceptionHandling(BinaryConvert.ToUtf8String);

    #endregion

    #region Type Specialized Primitive Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, byte> Any()
        => Any<byte>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, byte> Satisfy(Func<byte, bool> predicate)
        => Satisfy<byte>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, IReadOnlyList<byte>> Take(int count)
        => Take<byte>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, IReadOnlyCollection<byte>> TakeWhile(Func<byte, bool> predicate)
        => TakeWhile<byte>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, IReadOnlyCollection<byte>> TakeWhile1(Func<byte, bool> predicate)
        => TakeWhile1<byte>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> Skip(int count)
        => Skip<byte>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> SkipWhile(Func<byte, bool> predicate)
        => SkipWhile<byte>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> SkipWhile1(Func<byte, bool> predicate)
        => SkipWhile1<byte>(predicate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> EndOfInput()
        => EndOfInput<byte>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> Null()
        => Null<byte>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> Condition(bool success)
        => Condition<byte>(success);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, Unit> Condition(bool success, string message)
        => Condition<byte>(success, message);

    #endregion

    #region Type Specialized Monad Primitive Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Pure<T>(T value)
        => Pure<byte, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Pure<T>(Func<IParsecState<byte>, T> value)
        => Pure<byte, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Fail<T>()
        => Fail<byte, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Fail<T>(string message)
        => Fail<byte, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Fail<T>(Func<IParsecState<byte>, string> message)
        => Fail<byte, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Abort<T>(Func<IParsecState<byte>, string> message)
        => Abort<byte, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Abort<T>(Exception exception)
        => Abort<byte, T>(exception);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, IPosition> GetPosition()
        => GetPosition<byte>();

    #endregion

    #region Type Specialized Combinator Wrappers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<byte, T> Fix<T>(Func<IParser<byte, T>, IParser<byte, T>> function)
        => Parser.Fix(function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<TParameter, IParser<byte, T>> Fix<TParameter, T>(Func<Func<TParameter, IParser<byte, T>>, TParameter, IParser<byte, T>> function)
        => Parser.Fix(function);

    #endregion
}
