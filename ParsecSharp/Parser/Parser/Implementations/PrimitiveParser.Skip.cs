using System.Linq;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Skip<TToken> : PrimitiveParser<TToken, Unit>
    {
        private readonly int _count;

        public Skip(int count)
        {
            this._count = count;
        }

        protected sealed override Result<TToken, Unit> Run<TState>(TState state)
            => (state.AsEnumerable<TToken, TState>().Take(this._count).ToArray() is var result && result.Length == this._count)
                ? Result.Success<TToken, TState, Unit>(Unit.Instance, (result.Length == 0) ? state : result.Last().Next)
                : Result.Fail<TToken, TState, Unit>($"At {nameof(Skip<TToken>)} -> An input does not have enough length", state);
    }
}
