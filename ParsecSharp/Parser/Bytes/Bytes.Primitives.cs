using System.Runtime.CompilerServices;

namespace ParsecSharp
{
    public static partial class Bytes
    {
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
        public static IParser<byte, double> Double()
            => Take(sizeof(double)).Map(BinaryConvert.ToDouble);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, double> DoubleBigEndian()
            => Take(sizeof(double)).Map(BinaryConvert.ToDoubleBigEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, float> Single()
            => Take(sizeof(float)).Map(BinaryConvert.ToSingle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, float> SingleBigEndian()
            => Take(sizeof(float)).Map(BinaryConvert.ToSingleBigEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, sbyte> SByte()
            => Any().Map(x => (sbyte)x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, string> Utf8String(int count)
            => Take(count).MapWithExceptionHandling(BinaryConvert.ToUtf8String);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, string> Utf8StringBigEndian(int count)
            => Take(count).MapWithExceptionHandling(BinaryConvert.ToUtf8StringBigEndian);
    }
}
