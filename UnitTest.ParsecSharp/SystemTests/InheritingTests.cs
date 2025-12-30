using System;
using ParsecSharp;
using ParsecSharp.Internal;

namespace UnitTest.ParsecSharp.SystemTests;

// checks accessibility settings for user-inheriting
internal class InheritingTests
{
    private class PrimitiveParserInheritingTest<TToken, T> : PrimitiveParser<TToken, T>
    {
        protected override IResult<TToken, T> Run<TState>(TState state)
            => throw new NotImplementedException();

        public override string? ToString()
            => throw new NotImplementedException();
    }

    private class ModifyResultInheritingTest<TToken, T>(IParser<TToken, T> parser) : ModifyResult<TToken, T, T>(parser)
    {
        protected override IResult<TToken, T> Fail<TState>(TState state, IFailure<TToken, T> failure)
            => throw new NotImplementedException();

        protected override IResult<TToken, T> Succeed<TState>(TState state, ISuccess<TToken, T> success)
            => throw new NotImplementedException();

        public override string? ToString()
            => throw new NotImplementedException();
    }

    private class FailureInheritingTest<TToken, T> : Failure<TToken, T>
    {
        public override IParsecState<TToken> State => throw new NotImplementedException();

        public override ParsecSharpException Exception => throw new NotImplementedException();

        public override string Message => throw new NotImplementedException();

        protected override IFailure<TToken, TResult> Convert<TResult>()
            => throw new NotImplementedException();

        public override ISuspendedResult<TToken, T> Suspend()
            => throw new NotImplementedException();
    }
}
