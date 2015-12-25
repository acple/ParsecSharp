using System;

namespace Parsec
{
    public class ParsecException<T> : Exception
    {
        public IParsecState<T> State { get; }

        internal ParsecException(string message, IParsecState<T> state) : base(message)
        {
            this.State = state;
        }

        internal ParsecException(string message, Exception exception, IParsecState<T> state) : base(message, exception)
        {
            this.State = state;
        }
    }
}
