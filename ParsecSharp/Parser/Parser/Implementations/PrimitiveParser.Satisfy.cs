using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Satisfy<TToken> : PrimitiveParser<TToken, TToken>
    {
        private readonly Func<TToken, bool> _predicate;

        public Satisfy(Func<TToken, bool> predicate)
        {
            this._predicate = predicate;
        }

        protected sealed override Result<TToken, TToken> Run<TState>(TState state)
            => (state.HasValue && this._predicate(state.Current))
                ? Result.Success<TToken, TState, TToken>(state.Current, state.Next)
                : Result.Failure<TToken, TState, TToken>(state);
    }
}
