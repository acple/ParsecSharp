using System;
using Parsec.Internal;

namespace Parsec
{
    public abstract class Parser<TToken, T>
    {
        internal abstract Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont);

        public Result<TToken, T> Parse(IParsecStateStream<TToken> source)
        {
            using (source)
                return this.Run(source);
        }

        public ISuspendedResult<TToken, T> ParsePartially(IParsecStateStream<TToken> source)
            => this.Run(source);

        private Result<TToken, T> Run(IParsecStateStream<TToken> source)
        {
            try
            {
                return this.Run(source, result => result);
            }
            catch (Exception exception)
            {
                return new FailWithException<TToken, T>(exception, source);
            }
        }

        public static Parser<TToken, T> operator |(Parser<TToken, T> first, Parser<TToken, T> second)
            => new Alternative<TToken, T>(first, second);
    }
}
