using System;
using System.Runtime.InteropServices;

namespace ParsecSharp.Data;

[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
public readonly struct TextPosition(int line, int column) : IPosition<char, TextPosition>, IComparable<TextPosition>, IEquatable<TextPosition>
{
    public static TextPosition Initial => new();

    public int Line => line;

    public int Column => column;

    public TextPosition() : this(line: 1, column: 1)
    { }

    public TextPosition Next(char token)
        => token == '\n' ? new(line + 1, column: 1) : new(line, column + 1);

    public int CompareTo(IPosition? other)
        => other is null
            ? 1 // always greater than null
            : line != other.Line ? line.CompareTo(other.Line) : column.CompareTo(other.Column);

    public int CompareTo(TextPosition other)
        => line != other.Line ? line.CompareTo(other.Line) : column.CompareTo(other.Column);

    public bool Equals(IPosition? other)
        => other is TextPosition position && this == position;

    public bool Equals(TextPosition other)
        => this == other;

    public override bool Equals(object? obj)
        => obj is TextPosition position && this == position;

    public override int GetHashCode()
        => line ^ line << 16 ^ column;

    public override string ToString()
        => $"Line: {line.ToString()}, Column: {column.ToString()}";

    public static bool operator ==(TextPosition left, TextPosition right)
        => left.Line == right.Line && left.Column == right.Column;

    public static bool operator !=(TextPosition left, TextPosition right)
        => !(left == right);
}
