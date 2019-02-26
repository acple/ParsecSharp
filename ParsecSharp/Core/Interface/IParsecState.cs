using System;

namespace ParsecSharp
{
    public interface IParsecState<TToken> : IEquatable<IParsecState<TToken>>
    {
        TToken Current { get; }

        bool HasValue { get; }

        IPosition Position { get; }
    }
}
