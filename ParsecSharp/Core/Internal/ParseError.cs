using System;

namespace Parsec.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        public override ParsecException Exception => new ParsecException(this.ToString());

        internal ParseError(IParsecStateStream<TToken> state) : base(state)
        { }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> function)
            => new ParseError<TToken, TResult>(this.State);

        protected override string ToStringInternal()
            => $"Unexpected token '{ this.State }'";
    }
}
