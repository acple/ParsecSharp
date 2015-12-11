using System;

namespace Parsec
{
    [Serializable]
    public class ParsecException<TToken> : Exception
    {
        public IParsecState<TToken> State { get; }

        internal ParsecException(string message, IParsecState<TToken> state) : base(message)
        {
            this.State = state;
        }

        internal ParsecException(string message, Exception exception, IParsecState<TToken> state) : base(message, exception)
        {
            this.State = state;
        }
    }
}
