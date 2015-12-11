using System;

namespace Parsec.Internal
{
    internal class UserError<TToken, T> : Fail<TToken, T>
    {
        public override ParsecException<TToken> Exception { get; }

        internal UserError(ParsecException<TToken> exception) : base(exception.State)
        {
            this.Exception = exception;
        }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> _)
            => new UserError<TToken, TResult>(this.Exception);

        protected override string ToStringInternal()
            => this.Exception.Message;
    }
}
