using System;

namespace ParsecSharp
{
    public class ParsecException : Exception
    {
        public ParsecException(string message) : base(message)
        { }

        public ParsecException(string message, Exception exception) : base(message, exception)
        { }
    }
}
