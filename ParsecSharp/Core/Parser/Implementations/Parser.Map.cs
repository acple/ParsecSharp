using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Map<TToken, TIntermediate, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, TIntermediate> _parser;

        private readonly Func<TIntermediate, T> _function;

        public Map(Parser<TToken, TIntermediate> parser, Func<TIntermediate, T> function)
        {
            this._parser = parser;
            this._function = function;
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
        {
            var _function = this._function;
            return this._parser.Run(state, result => cont(result.Map(_function)));
        }
    }
}
