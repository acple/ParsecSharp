#if !NETSTANDARD1_6
using System;
using System.Runtime.Serialization;

namespace ParsecSharp
{
    [Serializable]
    public partial class ParsecException
    {
        protected ParsecException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
#endif
