using System.Runtime.CompilerServices;

namespace ParsecSharp.Internal
{
    internal static class CharConvert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToReadableStringWithCharCode(char token)
            => $"{ToReadableString(token)}<0x{((int)token).ToString("X2")}>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToReadableString(char token)
            => token switch
            {
                '\0' => "\\0",
                '\a' => "\\a",
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                '\v' => "\\v",
                _ => token.ToString(),
            };
    }
}
