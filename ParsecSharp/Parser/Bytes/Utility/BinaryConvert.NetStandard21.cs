#if NET || NETSTANDARD2_1_OR_GREATER
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ParsecSharp
{
    internal static class BinaryConvert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToInt16(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt16LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToInt16BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt16BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt32LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt32BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInt64(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt64LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInt64BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadInt64BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt16LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt16BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt32LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt32BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt64LittleEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64BigEndian(IEnumerable<byte> data)
            => BinaryPrimitives.ReadUInt64BigEndian([.. data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ToBoolean(byte data)
            => data != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToChar(IEnumerable<byte> data)
            => BitConverter.ToChar([.. BitConverter.IsLittleEndian ? data : data.Reverse()]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToCharBigEndian(IEnumerable<byte> data)
            => BitConverter.ToChar([.. BitConverter.IsLittleEndian ? data.Reverse() : data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(IEnumerable<byte> data)
            => BitConverter.ToDouble([.. BitConverter.IsLittleEndian ? data : data.Reverse()]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDoubleBigEndian(IEnumerable<byte> data)
            => BitConverter.ToDouble([.. BitConverter.IsLittleEndian ? data.Reverse() : data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToSingle(IEnumerable<byte> data)
            => BitConverter.ToSingle([.. BitConverter.IsLittleEndian ? data : data.Reverse()]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToSingleBigEndian(IEnumerable<byte> data)
            => BitConverter.ToSingle([.. BitConverter.IsLittleEndian ? data.Reverse() : data]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUtf8String(IEnumerable<byte> data)
            => Encoding.UTF8.GetString([.. BitConverter.IsLittleEndian ? data : data.Reverse()]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUtf8StringBigEndian(IEnumerable<byte> data)
            => Encoding.UTF8.GetString([.. BitConverter.IsLittleEndian ? data.Reverse() : data]);
    }
}
#endif
