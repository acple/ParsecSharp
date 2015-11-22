using System;

namespace Parsec.Internal
{
    internal class ParseError<TToken, T> : Fail<TToken, T>
    {
        private readonly string _name;

        public override ParsecException Exception => new ParsecException(this.ToString());

        internal ParseError(string name, IParsecStateStream<TToken> state) : base(state)
        {
            this._name = name;
        }

        internal override Result<TToken, TResult> Next<TResult>(Func<T, Parser<TToken, TResult>> function)
            => new ParseError<TToken, TResult>(this._name, this.State);

        protected override string ToStringInternal()
            => $"Unexpected token '{ this.State }' -- operator: { this._name }";
    }
}
