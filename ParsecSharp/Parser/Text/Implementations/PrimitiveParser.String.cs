using System;
using System.Linq;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class StringParser : PrimitiveParser<char, string>
    {
        private readonly string _text;

        private readonly StringComparison _comparison;

        public StringParser(string text, StringComparison comparison)
        {
            this._text = text;
            this._comparison = comparison;
        }

        protected sealed override Result<char, string> Run<TState>(TState state)
        {
            var states = ParsecState.AsEnumerable<char, TState>(state).Take(this._text.Length).ToArray();
            var result = new string(states.Select(x => x.Current).ToArray());
            return string.Equals(result, this._text, this._comparison)
                ? Result.Success<char, TState, string>(result, states.Length == 0 ? state : states.Last().Next)
                : Result.Failure<char, TState, string>($"Expected '{this._text}' but was '{result}'", state);
        }
    }
}
