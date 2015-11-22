using System;

namespace Parsec
{
    [Serializable]
    public class ParsecException : Exception
    {
        internal ParsecException(string message) : base(message)
        { }

        internal ParsecException(string message, Exception exception) : base(message, exception)
        { }
    }
}
