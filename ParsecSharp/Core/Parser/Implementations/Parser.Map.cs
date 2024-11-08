using System;

namespace ParsecSharp.Internal.Parsers
{
    internal sealed class Map<TToken, TIntermediate, T>(IParser<TToken, TIntermediate> parser, Func<TIntermediate, T> function) : Parser<TToken, T>
    {
        public sealed override IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            => parser.Run(state, result => cont(result.Map(function)));
    }
}
