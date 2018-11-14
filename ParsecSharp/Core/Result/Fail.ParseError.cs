namespace Parsec.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        internal ParseError(IParsecState<TToken> state) : base(state)
        { }

        protected override Fail<TToken, TNext> Convert<TNext>()
            => new ParseError<TToken, TNext>(this.State);

        protected override string ToStringInternal()
            => $"Unexpected \"{this.State.ToString()}\"";
    }
}
