using System;

namespace ParsecSharp;

public class ParsecSharpException : Exception
{
    public ParsecSharpException(string message) : base(message)
    { }

    public ParsecSharpException(string message, Exception innerException) : base(message, innerException)
    { }
}
