using System;
using Parsec.Internal;

namespace Parsec
{
    public class Parser<TToken, T>
    {
        private readonly Func<IParsecStateStream<TToken>, Result<TToken, T>> _function;

        internal Parser(Func<IParsecStateStream<TToken>, Result<TToken, T>> function)
        {
            this._function = function;
        }

        internal Result<TToken, T> Run(IParsecStateStream<TToken> state)
            => this._function(state);

        public Result<TToken, T> Parse(IParsecStateStream<TToken> source)
        {
            try
            {
                using (source)
                    return this.Run(source);
            }
            catch (Exception exception)
            {
                return Result.Fail<TToken, T>(exception, source);
            }
        }
    }
}
