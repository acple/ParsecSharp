using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParsecSharp
{
    public static partial class Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, short> Int16()
            => Take(sizeof(short)).Map(x => BitConverter.ToInt16(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, int> Int32()
            => Take(sizeof(int)).Map(x => BitConverter.ToInt32(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, long> Int64()
            => Take(sizeof(long)).Map(x => BitConverter.ToInt64(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, ushort> UInt16()
            => Take(sizeof(ushort)).Map(x => BitConverter.ToUInt16(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, uint> UInt32()
            => Take(sizeof(uint)).Map(x => BitConverter.ToUInt32(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, ulong> UInt64()
            => Take(sizeof(ulong)).Map(x => BitConverter.ToUInt64(x.ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, short> Int16BigEndian()
            => Take(sizeof(short)).Map(x => BitConverter.ToInt16(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, int> Int32BigEndian()
            => Take(sizeof(int)).Map(x => BitConverter.ToInt32(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, long> Int64BigEndian()
            => Take(sizeof(long)).Map(x => BitConverter.ToInt64(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, ushort> UInt16BigEndian()
            => Take(sizeof(ushort)).Map(x => BitConverter.ToUInt16(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, uint> UInt32BigEndian()
            => Take(sizeof(uint)).Map(x => BitConverter.ToUInt32(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, ulong> UInt64BigEndian()
            => Take(sizeof(ulong)).Map(x => BitConverter.ToUInt64(x.Reverse().ToArray(), 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, sbyte> SByte()
            => Any().Map(x => (sbyte)x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<byte, string> Utf8String(int count)
            => Take(count).Map(x => Encoding.UTF8.GetString(x.ToArray()));
    }
}
