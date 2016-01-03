using System;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, T> Return<TToken, T>(T value)
            => Builder.Create<TToken, T>(state => Result.Success(value, state));

        public static Parser<TToken, T> Return<TToken, T>(Func<T> valueFactory)
            => Builder.Create<TToken, T>(state => Result.Success(valueFactory(), state));

        public static Parser<TToken, T> Fail<TToken, T>()
            => Builder.Create<TToken, T>(state => Result.Fail<TToken, T>(state));

        public static Parser<TToken, T> Fail<TToken, T>(Func<IParsecState<TToken>, string> message)
            => Builder.Create<TToken, T>(state => Result.Fail<TToken, T>(message(state), state));

        public static Parser<TToken, T> Abort<TToken, T>(Func<IParsecState<TToken>, string> message)
            => Builder.Create<TToken, T>(state => { throw new ParsecException<TToken>(message(state), state); });

        public static Parser<T, IPosition> GetPosition<T>()
            => Builder.Create<T, IPosition>(state => Result.Success(state.Position, state));
    }
}
