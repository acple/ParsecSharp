using System;
using System.Runtime.InteropServices;

namespace ParsecSharp.Data
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct LinearPosition<TToken> : IPosition<TToken, LinearPosition<TToken>>, IComparable<LinearPosition<TToken>>, IEquatable<LinearPosition<TToken>>
    {
        public static LinearPosition<TToken> Initial => default;

        public int Line => 0;

        public int Column { get; }

        public LinearPosition(int index)
        {
            this.Column = index;
        }

        public LinearPosition<TToken> Next(TToken token)
            => new(this.Column + 1);

        public int CompareTo(IPosition? other)
            => other is null
                ? 1 // always greater than null
                : this.Line != other.Line ? this.Line.CompareTo(other.Line) : this.Column.CompareTo(other.Column);

        public int CompareTo(LinearPosition<TToken> other)
            => this.Column.CompareTo(other.Column);

        public bool Equals(IPosition? other)
            => other is LinearPosition<TToken> position && this == position;

        public bool Equals(LinearPosition<TToken> other)
            => this == other;

        public override bool Equals(object? obj)
            => obj is LinearPosition<TToken> position && this == position;

        public override int GetHashCode()
            => this.Column;

        public override string ToString()
            => $"Index: {this.Column.ToString()}";

        public static bool operator ==(LinearPosition<TToken> left, LinearPosition<TToken> right)
            => left.Column == right.Column;

        public static bool operator !=(LinearPosition<TToken> left, LinearPosition<TToken> right)
            => !(left == right);
    }
}
