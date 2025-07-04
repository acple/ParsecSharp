using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParsecSharp.Internal;

public sealed class Buffer<TToken>(TToken[] buffer, int offset, int length, Func<Buffer<TToken>> next) : IReadOnlyList<TToken>
{
    public static Buffer<TToken> Empty { get; } = new([], () => Empty!);

    private readonly Lazy<Buffer<TToken>> _next = new(next, LazyThreadSafetyMode.None);

    public TToken this[int index] => buffer[offset + index];

    public int Count => length;

    public Buffer<TToken> Next => this._next.Value;

    public Buffer(TToken[] buffer, Func<Buffer<TToken>> next) : this(buffer, offset: 0, buffer.Length, next)
    { }

    IEnumerator<TToken> IEnumerable<TToken>.GetEnumerator()
        => new ArraySegment<TToken>(buffer, offset, length).AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.AsEnumerable().GetEnumerator();
}
