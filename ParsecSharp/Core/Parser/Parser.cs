using System;
using System.Runtime.CompilerServices;
using ParsecSharp.Data;
using ParsecSharp.Internal;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public abstract class Parser<TToken, T>
    {
        private protected Parser()
        { }

        internal abstract Result<TToken, TResult> Run<TState, TResult>(TState state, Func<Result<TToken, T>, Result<TToken, TResult>> cont)
            where TState : IParsecState<TToken, TState>;

        public Result<TToken, T> Parse<TState>(TState source)
            where TState : IParsecState<TToken, TState>
        {
            using (source.InnerResource)
                return this.Run(ref source); // move parameter ownership here
        }

        public Result<TToken, T> Parse(ISuspendedState<TToken> suspended)
        {
            using (suspended.InnerResource)
                return suspended.Continue(this).Result;
        }

        public SuspendedResult<TToken, T> ParsePartially<TState>(TState source)
            where TState : IParsecState<TToken, TState>
            => this.Run(ref source).Suspend();

        public SuspendedResult<TToken, T> ParsePartially(ISuspendedState<TToken> suspended)
            => suspended.Continue(this);

        private Result<TToken, T> Run<TState>(ref TState source)
            where TState : IParsecState<TToken, TState>
        {
            try
            {
                return this.RunCore(ref source);
            }
            catch (Exception exception)
            {
                return Result.Failure<TToken, EmptyStream<TToken>, T>(exception, EmptyStream<TToken>.Instance);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Result<TToken, T> RunCore<TState>(ref TState source)
            where TState : IParsecState<TToken, TState>
            => this.Run(Move(ref source), result => result); // consume source here

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TReference Move<TReference>(ref TReference value)
        {
            var result = value;
            value = default!;
            return result;
        }

        public sealed override bool Equals(object? obj)
            => base.Equals(obj);

        public sealed override int GetHashCode()
            => base.GetHashCode();

        public static Parser<TToken, T> operator |(Parser<TToken, T> first, Parser<TToken, T> second)
            => new Alternative<TToken, T>(first, second);
    }
}
