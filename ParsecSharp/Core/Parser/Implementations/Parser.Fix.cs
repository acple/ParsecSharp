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

    internal sealed class Fix<TToken, TParam, T> : Parser<TToken, T>
    {
        private readonly Func<Func<TParam, Parser<TToken, T>>, TParam, Parser<TToken, T>> _function;

        private readonly TParam _parameter;

        public Fix(Func<Func<TParam, Parser<TToken, T>>, TParam, Parser<TToken, T>> function, TParam parameter)
        {
            this._function = function;
            this._parameter = parameter;
        }

        private Parser<TToken, T> Next(TParam parameter)
            => new Fix<TToken, TParam, T>(this._function, parameter);

        internal override Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            => this._function(this.Next, this._parameter).Run(state, cont);
    }
}
