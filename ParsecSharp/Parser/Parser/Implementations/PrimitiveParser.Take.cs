using System.Collections.Generic;
using System.Linq;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Take<TToken> : PrimitiveParser<TToken, IEnumerable<TToken>>
    {
        private readonly int _count;

        public Take(int count)
        {
            this._count = count;
        }

        protected sealed override Result<TToken, IEnumerable<TToken>> Run<TState>(TState state)
            => (state.AsEnumerable<TToken, TState>().Take(this._count).ToArray() is var result && result.Length == this._count)
                ? Result.Success<TToken, TState, IEnumerable<TToken>>(result.Select(x => x.Current).ToArray(), (result.Length == 0) ? state : result.Last().Next)
                : Result.Fail<TToken, TState, IEnumerable<TToken>>($"At {nameof(Take<TToken>)} -> An input does not have enough length", state);
    }
}
