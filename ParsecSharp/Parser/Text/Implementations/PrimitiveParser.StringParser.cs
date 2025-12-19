using System;
using System.Linq;

namespace ParsecSharp.Internal.Parsers;

internal sealed class StringParser(string text, StringComparison comparison) : PrimitiveParser<char, string>
{
    protected sealed override IResult<char, string> Run<TState>(TState state)
    {
        var states = ParsecState.AsEnumerable<char, TState>(state).Take(text.Length).ToArray();
        var result = new string([.. states.Select(x => x.Current)]);
        return string.Equals(result, text, comparison)
            ? Result.Success<char, TState, string>(result, states.Length == 0 ? state : states.Last().Next)
            : Result.Failure<char, TState, string>($"Expected '{text}' but was '{result}'", state);
    }
}
