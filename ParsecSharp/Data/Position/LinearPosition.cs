using System;
using System.Runtime.InteropServices;

namespace ParsecSharp.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 4)]
    public readonly struct LinearPosition<TToken>(int index) : IPosition<TToken, LinearPosition<TToken>>, IComparable<LinearPosition<TToken>>, IEquatable<LinearPosition<TToken>>
    {
        public static LinearPosition<TToken> Initial => default;

        public int Line => 0;

        public int Column => index;

        public LinearPosition<TToken> Next(TToken token)
            => new(index + 1);

        public int CompareTo(IPosition? other)
            => other is null
                ? 1 // always greater than null
                : other.Line != 0 ? -other.Line : index.CompareTo(other.Column);

        public int CompareTo(LinearPosition<TToken> other)
            => index.CompareTo(other.Column);

        public bool Equals(IPosition? other)
            => other is LinearPosition<TToken> position && this == position;

        public bool Equals(LinearPosition<TToken> other)
            => this == other;

        public override bool Equals(object? obj)
            => obj is LinearPosition<TToken> position && this == position;

        public override int GetHashCode()
            => index;

        public override string ToString()
            => $"Index: {index.ToString()}";

        public static bool operator ==(LinearPosition<TToken> left, LinearPosition<TToken> right)
            => left.Column == right.Column;

        public static bool operator !=(LinearPosition<TToken> left, LinearPosition<TToken> right)
            => !(left == right);
    }
}
