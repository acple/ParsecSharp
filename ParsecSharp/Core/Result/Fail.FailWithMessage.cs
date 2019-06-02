namespace ParsecSharp.Internal
{
    internal sealed class FailWithMessage<TToken, T> : Fail<TToken, T>
    {
        public sealed override string Message { get; }

        internal FailWithMessage(string message, IParsecStateStream<TToken> state) : base(state)
        {
            this.Message = message;
        }

        public sealed override Fail<TToken, TNext> Convert<TNext>()
            => new FailWithMessage<TToken, TNext>(this.Message, this.Rest);
    }
}
