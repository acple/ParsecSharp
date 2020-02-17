namespace ParsecSharp.Internal
{
    internal static class ClosureOptimization
    {
        public static T Const<T, TIgnore>(this T value, TIgnore _)
            where T : class
            => value;
    }
}
