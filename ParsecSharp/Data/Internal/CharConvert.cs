namespace ParsecSharp.Internal
{
    internal static class CharConvert
    {
        public static string ToReadableStringWithCharCode(this char token)
            => $"{token.ToReadableString()}<0x{((int)token).ToString("X2")}>";

        public static string ToReadableString(this char token)
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
