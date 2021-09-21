using System;
using System.Runtime.Serialization;

namespace ParsecSharp
{
    [Serializable]
    public class ParsecException : Exception
    {
        public ParsecException(string message) : base(message)
        { }

        public ParsecException(string message, Exception innerException) : base(message, innerException)
        { }

        protected ParsecException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
