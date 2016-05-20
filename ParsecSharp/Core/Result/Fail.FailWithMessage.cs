namespace Parsec.Internal
{
    internal class FailWithMessage<TToken, T> : Fail<TToken, T>
    {
        private readonly string _message;

        internal FailWithMessage(string message, IParsecState<TToken> state) : base(state)
        {
            this._message = message;
        }

        protected override Fail<TToken, TResult> Next<TResult>()
            => new FailWithMessage<TToken, TResult>(this._message, this.State);

        protected override string ToStringInternal()
            => this._message;
    }
}
