namespace ParsecSharp.Internal
{
    internal class FailWithMessage<TToken, T> : Fail<TToken, T>
    {
        private readonly string _message;

        internal FailWithMessage(string message, IParsecStateStream<TToken> state) : base(state)
        {
            this._message = message;
        }

        protected override Fail<TToken, TNext> Convert<TNext>()
            => new FailWithMessage<TToken, TNext>(this._message, this.Rest);

        protected override string ToStringInternal()
            => this._message;
    }
}
