using System;

namespace Parsec.Internal
{
    internal class FailWithException<TToken, T> : Fail<TToken, T>
    {
        private readonly Exception _exception;

        public override ParsecException<TToken> Exception => new ParsecException<TToken>(this.ToString(), this._exception, this.State);

        internal FailWithException(Exception exception, IParsecState<TToken> state) : base(state)
        {
            this._exception = exception;
        }

        protected override Result<TToken, TResult> Next<TResult>()
            => new FailWithException<TToken, TResult>(this._exception, this.State);

        protected override string ToStringInternal()
            => $"External Error '{ this._exception.GetType().Name }' occurred: { this._exception }";
    }
}
