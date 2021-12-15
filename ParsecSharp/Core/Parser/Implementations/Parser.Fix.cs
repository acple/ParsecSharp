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

    internal sealed class Fix<TToken, TParamater, T> : Parser<TToken, T>
    {
        private readonly Func<Func<TParamater, Parser<TToken, T>>, TParamater, Parser<TToken, T>> _function;

        private readonly TParamater _parameter;

        public Fix(Func<Func<TParamater, Parser<TToken, T>>, TParamater, Parser<TToken, T>> function, TParamater parameter)
        {
            this._function = function;
            this._parameter = parameter;
        }

        internal override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._function(parameter => new Fix<TToken, TParamater, T>(this._function, parameter), this._parameter).Run(state, cont);
    }
}
