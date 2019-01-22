using System;

namespace ParsecSharp.Internal
{
    internal sealed class FailWithException<TToken, T> : Fail<TToken, T>
    {
        private readonly Exception _exception;

        public sealed override ParsecException Exception => new ParsecException(this.ToString(), this._exception);

        internal FailWithException(Exception exception, IParsecStateStream<TToken> state) : base(state)
        {
            this._exception = exception;
        }

        protected sealed override Fail<TToken, TNext> Convert<TNext>()
            => new FailWithException<TToken, TNext>(this._exception, this.Rest);

        protected sealed override string ToStringInternal()
            => $"Exception '{this._exception.GetType().Name}' occurred: {this._exception.ToString()}";
    }
}
