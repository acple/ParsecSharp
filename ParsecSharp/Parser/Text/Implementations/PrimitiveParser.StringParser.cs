using System;
using System.Linq;
using System.Text;

namespace ParsecSharp.Internal.Parsers;

internal sealed class StringParser(string text, StringComparison comparison) : PrimitiveParser<char, string>
{
    protected sealed override IResult<char, string> Run<TState>(TState state)
    {
        var (builder, last) = ParsecState.AsEnumerable<char, TState>(state)
            .Take(text.Length)
            .Aggregate((builder: new StringBuilder(text.Length), last: state),
                (accumulator, state) => (accumulator.builder.Append(state.Current), state.Next));
        var result = builder.ToString();
        return string.Equals(result, text, comparison)
            ? Result.Success<char, TState, string>(result, last)
            : Result.Failure<char, TState, string>($"Expected '{text}' but was '{result}'", state);
    }
}
