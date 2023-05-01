namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Not<TToken, TIgnore, T> : ModifyResult<TToken, TIgnore, T>
    {
        private readonly T _result;

        public Not(Parser<TToken, TIgnore> parser, T result) : base(parser)
        {
            this._result = result;
        }

        protected sealed override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, TIgnore> failure)
            => Result.Success<TToken, TState, T>(this._result, state);

        protected sealed override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, TIgnore> success)
            => Result.Failure<TToken, TState, T>($"Unexpected succeed with value '{success.ToString()}'", state);
    }
}
