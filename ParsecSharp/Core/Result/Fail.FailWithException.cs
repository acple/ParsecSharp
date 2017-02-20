using System;

namespace Parsec.Internal
{
    internal class FailWithException<TToken, T> : Fail<TToken, T>
    {
        private readonly Exception _exception;

        public override ParsecException Exception => new ParsecException(this.ToString(), this._exception);

        internal FailWithException(Exception exception, IParsecState<TToken> state) : base(state)
        {
            this._exception = exception;
        }

        protected override Fail<TToken, TNext> Next<TNext>()
            => new FailWithException<TToken, TNext>(this._exception, this.State);

        protected override string ToStringInternal()
            => $"Exception '{this._exception.GetType().Name}' occurred: {this._exception}";
    }
}
