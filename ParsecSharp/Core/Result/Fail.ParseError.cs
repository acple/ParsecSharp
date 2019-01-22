namespace ParsecSharp.Internal
{
    internal sealed class ParseError<TToken, T> : Fail<TToken, T>
    {
        internal ParseError(IParsecStateStream<TToken> state) : base(state)
        { }

        protected sealed override Fail<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TNext>(this.Rest);

        protected sealed override string ToStringInternal()
            => $"Unexpected \"{this.State.ToString()}\"";
    }
}
