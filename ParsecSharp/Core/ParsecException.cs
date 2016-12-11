using System;

namespace Parsec
{
    public class ParsecException<TToken> : Exception
    {
        public IParsecState<TToken> State { get; }

        public ParsecException(string message, IParsecState<TToken> state) : base(message)
        {
            this.State = state;
        }

        public ParsecException(string message, Exception exception, IParsecState<TToken> state) : base(message, exception)
        {
            this.State = state;
        }
    }
}
