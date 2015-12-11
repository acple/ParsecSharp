using System;

namespace Parsec.Internal
{
    internal class FailWithMessage<TToken, T> : Fail<TToken, T>
    {
        private readonly string _message;

        public override ParsecException<TToken> Exception => new ParsecException<TToken>(this.ToString(), this.State);

        internal FailWithMessage(string message, IParsecState<TToken> state) : base(state)
        {
            this._message = message;
        }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> _)
            => new FailWithMessage<TToken, TResult>(this._message, this.State);

        protected override string ToStringInternal()
            => this._message;
    }
}
