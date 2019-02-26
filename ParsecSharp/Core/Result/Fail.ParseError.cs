namespace ParsecSharp.Internal
{
    internal sealed class ParseError<TToken, T> : Fail<TToken, T>
    {
        public sealed override string Message => $"Unexpected '{this.State.ToString()}'";

        internal ParseError(IParsecStateStream<TToken> state) : base(state)
        { }

        protected sealed override Fail<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TNext>(this.Rest);
    }
}
