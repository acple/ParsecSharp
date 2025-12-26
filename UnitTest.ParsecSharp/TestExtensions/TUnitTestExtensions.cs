using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;
using TUnit.Assertions.Enums;

namespace UnitTest.ParsecSharp;

internal static class TUnitTestExtensions
{
    [SuppressMessage("Usage", "TUnitAssertions0003:Manual CallerArgumentExpression parameter provided")]
    public static IsEquivalentToAssertion<TCollection, TItem> IsSequentiallyEqualTo<TCollection, TItem>(this IAssertionSource<TCollection> source, IEnumerable<TItem> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
        where TCollection : IEnumerable<TItem>
        => source.IsEquivalentTo(expected, CollectionOrdering.Matching, expectedExpression: expectedExpression);
}
