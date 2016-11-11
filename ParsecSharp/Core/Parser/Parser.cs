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
            {
                try
                {
                    return this.Run(source, result => result);
                }
                catch (ParsecException<TToken> exception)
                {
                    return new UserError<TToken, T>(exception);
                }
                catch (Exception exception)
                {
                    return new FailWithException<TToken, T>(exception, source);
                }
            }
        }
    }
}
