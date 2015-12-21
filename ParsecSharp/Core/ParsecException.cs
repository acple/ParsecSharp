using System;
using System.Runtime.Serialization;

namespace Parsec
{
    [Serializable]
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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.State), this.State, typeof(IParsecState<T>));
            base.GetObjectData(info, context);
        }
    }
}
