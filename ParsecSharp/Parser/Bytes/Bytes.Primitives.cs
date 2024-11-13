using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParsecSharp
{
    public static partial class Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, short> Int16()
            => Take(sizeof(short)).Map(bytes => BitConverter.ToInt16([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, int> Int32()
            => Take(sizeof(int)).Map(bytes => BitConverter.ToInt32([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, long> Int64()
            => Take(sizeof(long)).Map(bytes => BitConverter.ToInt64([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, ushort> UInt16()
            => Take(sizeof(ushort)).Map(bytes => BitConverter.ToUInt16([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, uint> UInt32()
            => Take(sizeof(uint)).Map(bytes => BitConverter.ToUInt32([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, ulong> UInt64()
            => Take(sizeof(ulong)).Map(bytes => BitConverter.ToUInt64([.. bytes], 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, short> Int16BigEndian()
            => Take(sizeof(short)).Map(bytes => BitConverter.ToInt16(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, int> Int32BigEndian()
            => Take(sizeof(int)).Map(bytes => BitConverter.ToInt32(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, long> Int64BigEndian()
            => Take(sizeof(long)).Map(bytes => BitConverter.ToInt64(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, ushort> UInt16BigEndian()
            => Take(sizeof(ushort)).Map(bytes => BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, uint> UInt32BigEndian()
            => Take(sizeof(uint)).Map(bytes => BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, ulong> UInt64BigEndian()
            => Take(sizeof(ulong)).Map(bytes => BitConverter.ToUInt64(bytes.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, sbyte> SByte()
            => Any().Map(x => (sbyte)x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<byte, string> Utf8String(int count)
            => Take(count).Map(bytes => Encoding.UTF8.GetString([.. bytes]));
    }
}
