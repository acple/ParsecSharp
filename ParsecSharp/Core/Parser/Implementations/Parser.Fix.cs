using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Fix<TToken, T> : Parser<TToken, T>
    {
        private readonly Parser<TToken, T> _parser;

        public Fix(Func<Parser<TToken, T>, Parser<TToken, T>> function)
        {
            this._parser = function(this);
        }

        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._parser.Run(state, cont);
    }

    internal sealed class Fix<TToken, TParamater, T>(Func<Func<TParamater, Parser<TToken, T>>, TParamater, Parser<TToken, T>> function, TParamater parameter) : Parser<TToken, T>
    {
        internal sealed override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => function(parameter => new Fix<TToken, TParamater, T>(function, parameter), parameter).Run(state, cont);
    }
}
