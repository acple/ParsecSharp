namespace ParsecSharp.Internal.Parsers
{
    internal sealed class GetPosition<TToken> : PrimitiveParser<TToken, IPosition>
    {
        protected sealed override Result<TToken, IPosition> Run<TState>(TState state)
            => Result.Success<TToken, TState, IPosition>(state.Position, state);
    }
}
