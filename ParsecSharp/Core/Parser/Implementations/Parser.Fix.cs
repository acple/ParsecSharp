using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Fix<TToken, T> : Parser<TToken, T>
    {
        private readonly IParser<TToken, T> _parser;

        public Fix(Func<IParser<TToken, T>, IParser<TToken, T>> function)
        {
            this._parser = function(this);
        }

        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => this._parser.Run(state, cont);
    }

    internal sealed class Fix<TToken, TParamater, T>(Func<Func<TParamater, IParser<TToken, T>>, TParamater, IParser<TToken, T>> function, TParamater parameter) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => function(parameter => new Fix<TToken, TParamater, T>(function, parameter), parameter).Run(state, cont);
    }
}
