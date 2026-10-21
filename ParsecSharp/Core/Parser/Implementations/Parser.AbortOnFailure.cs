using System;
using ParsecSharp.Internal.Results;

namespace ParsecSharp.Internal.Parsers;

/// <summary>
/// Runs a parser and, when it fails, ends the whole parse with that failure
/// instead of letting an enclosing combinator recover from it.
/// </summary>
/// <param name="parser">The parser to run.</param>
/// <param name="message">Builds the message of the aborted result from the failure.</param>
/// <param name="onlyIfEntered">
/// Selects between the two public combinators.
/// <see langword="false"/> is <c>AbortWhenFail</c>, which aborts on any failure.
/// <see langword="true"/> is <c>AbortIfEntered</c>, which aborts only when the parser
/// consumed input before failing, and otherwise fails normally.
/// </param>
internal sealed class AbortOnFailure<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message, bool onlyIfEntered) : IParser<TToken, T>
{
    IResult<TToken, TResult> IParser<TToken, T>.Run<TState, TResult>(TState state, Func<IResult<TToken, T>, IResult<TToken, TResult>> cont)
        => parser.Run(state, result => result.CaseOf(
            // "state" is where the parser started and "failure.State" is where
            // it failed, so equal positions mean it failed without consuming
            // any input.
            failure => onlyIfEntered && state.Position.Equals(failure.State.Position)
                // Not entered: hand the failure to the continuation unchanged,
                // so that alternatives such as Or can still be tried.
                ? cont(failure)
                // Abort: return without calling the continuation. Nothing after
                // this point runs, so nothing can recover, and this result
                // becomes the result of the whole parse.
                //
                // The result wraps the original failure and replaces only its
                // message. Its State is therefore the state the parser failed
                // at, so the reported position is where the input went wrong,
                // not where this parser began.
                : new FailureOverrideMessage<TToken, T, TResult>(failure, message(failure)),
            // Success is passed on untouched.
            cont));
}
