using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public abstract class Parser<TToken, T>
    {
        internal abstract Result<TToken, TResult> Run<TResult>(IParsecStateStream<TToken> state, Func<Result<TToken, T>, Result<TToken, TResult>> cont);

        public Result<TToken, T> Parse(IParsecStateStream<TToken> source)
        {
            using (source.InnerResource)
                return this.Run(ref source); // move parameter ownership here
        }

        public Result<TToken, T>.Suspended ParsePartially(IParsecStateStream<TToken> source)
            => this.Run(ref source).Suspend();

        private Result<TToken, T> Run(ref IParsecStateStream<TToken> source)
        {
            try
            {
                return this.RunCore(ref source);
            }
            catch (Exception exception)
            {
                return new FailWithException<TToken, T>(exception, EmptyStream<TToken>.Instance);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Result<TToken, T> RunCore(ref IParsecStateStream<TToken> source)
            => this.Run(Interlocked.Exchange(ref source, default!), result => result); // consume source here

        public static Parser<TToken, T> operator |(Parser<TToken, T> first, Parser<TToken, T> second)
            => new Alternative<TToken, T>(first, second);
    }
}
