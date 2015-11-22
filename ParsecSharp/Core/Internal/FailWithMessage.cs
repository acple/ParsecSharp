using System;

namespace Parsec.Internal
{
    internal class FailWithMessage<TToken, T> : Fail<TToken, T>
    {
        private readonly string _message;

        public override ParsecException Exception => new ParsecException(this.ToString());

        internal FailWithMessage(string message, IParsecStateStream<TToken> state) : base(state)
        {
            this._message = message;
        }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> function)
            => new FailWithMessage<TToken, TResult>(this._message, this.State);

        protected override string ToStringInternal()
            => this._message;
    }
}
