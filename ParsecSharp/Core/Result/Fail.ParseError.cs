namespace ParsecSharp.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        internal ParseError(IParsecStateStream<TToken> state) : base(state)
        { }

        protected override Fail<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TNext>(this.Rest);

        protected override string ToStringInternal()
            => $"Unexpected \"{this.State.ToString()}\"";
    }
}
