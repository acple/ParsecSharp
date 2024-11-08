using System;
using ParsecSharp.Data;

namespace ParsecSharp.Internal
{
    public abstract class Parser<TToken, T> : IParser<TToken, T>
    {
        public IResult<TToken, T> Parse<TState>(TState source)
            where TState : IParsecState<TToken, TState>
        {
            using (source.InnerResource)
                return this.Run(source);
        }

        public IResult<TToken, T> Parse(ISuspendedState<TToken> suspended)
        {
            using (suspended.InnerResource)
                return suspended.Continue(this).Result;
        }

        public ISuspendedResult<TToken, T> ParsePartially<TState>(TState source)
            where TState : IParsecState<TToken, TState>
            => this.Run(source).Suspend();

        public ISuspendedResult<TToken, T> ParsePartially(ISuspendedState<TToken> suspended)
            => suspended.Continue(this);

        private IResult<TToken, T> Run<TState>(TState source)
            where TState : IParsecState<TToken, TState>
        {
            try
            {
                return this.Run(source, result => result);
            }
            catch (Exception exception)
            {
                return Result.Failure<TToken, EmptyStream<TToken>, T>(exception, EmptyStream<TToken>.Instance);
            }
        }

        public abstract IResult<TToken, TResult> Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
            where TState : IParsecState<TToken, TState>;

        public sealed override bool Equals(object? obj)
            => base.Equals(obj);

        public sealed override int GetHashCode()
            => base.GetHashCode();
    }
}
