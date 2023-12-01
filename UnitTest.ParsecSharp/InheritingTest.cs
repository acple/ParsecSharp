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
            protected override Result<TToken, T> Run<TState>(TState state)
                => throw new NotImplementedException();

            public override string? ToString()
                => base.ToString();
        }

        private class ModifyResultInheritingTest<TToken, T>(Parser<TToken, T> parser) : ModifyResult<TToken, T, T>(parser)
        {
            protected override Result<TToken, T> Fail<TState>(TState state, Failure<TToken, T> failure)
                => throw new NotImplementedException();

            protected override Result<TToken, T> Succeed<TState>(TState state, Success<TToken, T> success)
                => throw new NotImplementedException();

            public override string? ToString()
                => base.ToString();
        }

        private class SuccessInheritingTest<TToken, T>(T result) : Success<TToken, T>(result)
        {
            public override T Value => base.Value;

            public override Result<TToken, TResult> Map<TResult>(Func<T, TResult> function)
                => throw new NotImplementedException();

            protected override SuspendedResult<TToken, T> Suspend()
                => throw new NotImplementedException();

            protected override Result<TToken, TResult> RunNext<TNext, TResult>(Parser<TToken, TNext> parser, Func<Result<TToken, TNext>, Result<TToken, TResult>> cont)
                => throw new NotImplementedException();

            public override string ToString()
                => base.ToString();
        }

        private class FailureInheritingTest<TToken, T> : Failure<TToken, T>
        {
            public override IParsecState<TToken> State => throw new NotImplementedException();

            public override ParsecException Exception => base.Exception;

            public override string Message => throw new NotImplementedException();

            public override Failure<TToken, TNext> Convert<TNext>()
                => throw new NotImplementedException();

            protected override SuspendedResult<TToken, T> Suspend()
                => throw new NotImplementedException();
        }
    }
}
