using System;
using static Parsec.Parser;

namespace Parsec
{
    public static partial class Text
    {
        public static Parser<char, T> Pure<T>(T value)
            => Pure<char, T>(value);

        public static Parser<char, T> Pure<T>(Func<T> value)
            => Pure<char, T>(value);

        public static Parser<char, T> Fail<T>()
            => Fail<char, T>();

        public static Parser<char, T> Fail<T>(Func<IParsecState<char>, string> message)
            => Fail<char, T>(message);

        public static Parser<char, T> Abort<T>(Func<IParsecState<char>, string> message)
            => Abort<char, T>(message);

        public static Parser<char, T> Abort<T>(Exception exception)
            => Abort<char, T>(exception);

        public static Parser<char, IPosition> GetPosition()
            => GetPosition<char>();
    }
}
