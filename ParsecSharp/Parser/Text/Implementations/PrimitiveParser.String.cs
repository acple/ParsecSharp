using System;
using System.Linq;

namespace ParsecSharp.Internal.Parsers
{
    internal class ParserString : PrimitiveParser<char, string>
    {
        private readonly string _text;

        private readonly StringComparison _comparison;

        public ParserString(string text, StringComparison comparison)
        {
            this._text = text;
            this._comparison = comparison;
        }

        protected sealed override Result<char, string> Run<TState>(TState state)
        {
            var result = state.AsEnumerable<char, TState>().Take(this._text.Length).ToArray();
            var text = new string(result.Select(x => x.Current).ToArray());
            return (string.Equals(text, this._text, this._comparison))
                ? Result.Success<char, TState, string>(text, (result.Length == 0) ? state : result.Last().Next)
                : Result.Failure<char, TState, string>($"Expected '{this._text}' but was '{text}'", state);
        }
    }
}
