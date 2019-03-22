using System;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public abstract class Parser<TToken, T>
    {
        protected internal abstract Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont);

        public Result<TToken, T> Parse(IParsecStateStream<TToken> source)
        {
            using (source)
                return this.Run(source);
        }

        public Result<TToken, T>.Suspended ParsePartially(IParsecStateStream<TToken> source)
            => this.Run(source).Suspend();

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
