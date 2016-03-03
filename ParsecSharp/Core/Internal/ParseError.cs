namespace Parsec.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        internal ParseError(IParsecState<TToken> state) : base(state)
        { }

        protected override Fail<TToken, TResult> Next<TResult>()
            => new ParseError<TToken, TResult>(this.State);

        protected override string ToStringInternal()
            => $"Unexpected \"{ this.State }\"";
    }
}
