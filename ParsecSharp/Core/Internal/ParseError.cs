using System;

namespace Parsec.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        public override ParsecException<TToken> Exception => new ParsecException<TToken>(this.ToString(), this.State);

        internal ParseError(IParsecState<TToken> state) : base(state)
        { }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> _)
            => new ParseError<TToken, TResult>(this.State);

        protected override string ToStringInternal()
            => $"Unexpected \"{ this.State }\"";
    }
}
