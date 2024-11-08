using System;
using ParsecSharp;
using ParsecSharp.Internal;

namespace UnitTest.ParsecSharp
{
    // checks accessibility settings for user-inheriting
    internal class InheritingTest
    {
        private class PrimitiveParserInheritingTest<TToken, T> : PrimitiveParser<TToken, T>
        {
            protected override IResult<TToken, T> Run<TState>(TState state)
                => throw new NotImplementedException();

            public override string? ToString()
                => base.ToString();
        }

        private class ModifyResultInheritingTest<TToken, T>(Parser<TToken, T> parser) : ModifyResult<TToken, T, T>(parser)
        {
            protected override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
                => throw new NotImplementedException();

            protected override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
                => throw new NotImplementedException();

            public override string? ToString()
                => base.ToString();
        }

        private class SuccessInheritingTest<TToken, T>(T result) : Success<TToken, T>(result)
        {
            public override IResult<TToken, TResult> Map<TResult>(Func<T, TResult> function)
                => throw new NotImplementedException();

            public override IResult<TToken, TResult> Next<TNext, TResult>(Func<T, IParser<TToken, TNext>> next, Func<IResult<TToken, TNext>, IResult<TToken, TResult>> cont)
                => throw new NotImplementedException();

            public override SuspendedResult<TToken, T> Suspend()
                => throw new NotImplementedException();
        }

        private class FailureInheritingTest<TToken, T> : Failure<TToken, T>
        {
            public override IParsecState<TToken> State => throw new NotImplementedException();

            public override ParsecException Exception => base.Exception;

            public override string Message => throw new NotImplementedException();

            public override Failure<TToken, TNext> Convert<TNext>()
                => throw new NotImplementedException();

            public override SuspendedResult<TToken, T> Suspend()
                => throw new NotImplementedException();
        }
    }
}
