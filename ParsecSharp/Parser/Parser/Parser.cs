using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp;

public static class Parser
{
    #region Parser Extensions

    extension<TToken, T>(IParser<TToken, T> parser)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse(IReadOnlyList<TToken> source)
            => parser.Parse(ArrayStream.Create(source));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IResult<TToken, T> Parse(IEnumerable<TToken> source)
            => parser.Parse(EnumerableStream.Create(source));
    }

    #endregion

    #region Token Matching Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TToken> Any<TToken>()
        => Satisfy<TToken>(_ => true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TToken> Token<TToken>(TToken token)
        => Satisfy<TToken>(x => EqualityComparer<TToken>.Default.Equals(x, token));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TToken> OneOf<TToken>(params IEnumerable<TToken> candidates)
        => Satisfy<TToken>(candidates.Contains);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, TToken> OneOf<TToken>(params TToken[] candidates)
        => OneOf(candidates.AsEnumerable());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TToken> NoneOf<TToken>(params IEnumerable<TToken> candidates)
        => Satisfy<TToken>(x => !candidates.Contains(x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, TToken> NoneOf<TToken>(params TToken[] candidates)
        => NoneOf(candidates.AsEnumerable());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TToken> Satisfy<TToken>(Func<TToken, bool> predicate)
        => new Satisfy<TToken>(predicate);

    #endregion

    #region Token Sequence Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<TToken>> Take<TToken>(int count)
        => new Take<TToken>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> TakeWhile<TToken>(Func<TToken, bool> predicate)
        => Many(Satisfy(predicate));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> TakeWhile1<TToken>(Func<TToken, bool> predicate)
        => Many1(Satisfy(predicate));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Skip<TToken>(int count)
        => new Skip<TToken>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> SkipWhile<TToken>(Func<TToken, bool> predicate)
        => SkipMany(Satisfy(predicate));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> SkipWhile1<TToken>(Func<TToken, bool> predicate)
        => SkipMany1(Satisfy(predicate));

    #endregion

    #region Stream Control Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> EndOfInput<TToken>()
        => Not(Any<TToken>()).WithMessage(failure => $"Expected '<EndOfStream>' but was '{failure.State.ToString()}'");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Null<TToken>()
        => Pure<TToken, Unit>(Unit.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Condition<TToken>(bool success)
        => Condition<TToken>(success, "Given condition was false");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Condition<TToken>(bool success, string message)
        => success ? Null<TToken>() : Fail<TToken, Unit>(message);

    #endregion

    #region Monad Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Pure<TToken, T>(T value)
        => new Pure<TToken, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Pure<TToken, T>(Func<IParsecState<TToken>, T> value)
        => new PureDelayed<TToken, T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Fail<TToken, T>()
        => new Fail<TToken, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Fail<TToken, T>(string message)
        => new FailWithMessage<TToken, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Fail<TToken, T>(Func<IParsecState<TToken>, string> message)
        => new FailWithMessageDelayed<TToken, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Abort<TToken, T>(Func<IParsecState<TToken>, string> message)
        => new Terminate<TToken, T>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Abort<TToken, T>(Exception exception)
        => new Abort<TToken, T>(exception);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IPosition> GetPosition<TToken>()
        => new GetPosition<TToken>();

    #endregion

    #region Parser Selection Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Choice<TToken, T>(params IEnumerable<IParser<TToken, T>> parsers)
        => parsers.Reverse().Aggregate((next, parser) => parser.Alternative(next));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, T> Choice<TToken, T>(params IParser<TToken, T>[] parsers)
        => Choice(parsers.AsEnumerable());

    #endregion

    #region Parser Fallback Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Try<TToken, T>(IParser<TToken, T> parser, T fallback)
        => parser.Alternative(Pure<TToken, T>(fallback));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Try<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, T> fallback)
        => new Try<TToken, T>(parser, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, bool> Optional<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
        => parser.Either(_ => true, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Optional<TToken, T>(IParser<TToken, T> parser, T fallback)
        => Try(parser, fallback);

    #endregion

    #region Parser Sequencing Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Sequence<TToken, T>(params IEnumerable<IParser<TToken, T>> parsers)
        => parsers.Reverse()
            .Aggregate(Pure<TToken, IReadOnlyCollection<T>>([]),
                (next, parser) => parser.Bind(x => next.Map(result => result.Prepend(x))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, IReadOnlyCollection<T>> Sequence<TToken, T>(params IParser<TToken, T>[] parsers)
        => Sequence(parsers.AsEnumerable());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2)
        => Sequence(parser1, parser2, (a, b) => (IReadOnlyList<T>)[a, b]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3)
        => Sequence(parser1, parser2, parser3, (a, b, c) => (IReadOnlyList<T>)[a, b, c]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3, IParser<TToken, T> parser4)
        => Sequence(parser1, parser2, parser3, parser4, (a, b, c, d) => (IReadOnlyList<T>)[a, b, c, d]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3, IParser<TToken, T> parser4, IParser<TToken, T> parser5)
        => Sequence(parser1, parser2, parser3, parser4, parser5, (a, b, c, d, e) => (IReadOnlyList<T>)[a, b, c, d, e]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3, IParser<TToken, T> parser4, IParser<TToken, T> parser5, IParser<TToken, T> parser6)
        => Sequence(parser1, parser2, parser3, parser4, parser5, parser6, (a, b, c, d, e, f) => (IReadOnlyList<T>)[a, b, c, d, e, f]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3, IParser<TToken, T> parser4, IParser<TToken, T> parser5, IParser<TToken, T> parser6, IParser<TToken, T> parser7)
        => Sequence(parser1, parser2, parser3, parser4, parser5, parser6, parser7, (a, b, c, d, e, f, g) => (IReadOnlyList<T>)[a, b, c, d, e, f, g]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Sequence<TToken, T>(IParser<TToken, T> parser1, IParser<TToken, T> parser2, IParser<TToken, T> parser3, IParser<TToken, T> parser4, IParser<TToken, T> parser5, IParser<TToken, T> parser6, IParser<TToken, T> parser7, IParser<TToken, T> parser8)
        => Sequence(parser1, parser2, parser3, parser4, parser5, parser6, parser7, parser8, (a, b, c, d, e, f, g, h) => (IReadOnlyList<T>)[a, b, c, d, e, f, g, h]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, Func<T1, T2, TResult> selector)
        => parser1.Bind(a => parser2.Map(b => selector(a, b)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, Func<T1, T2, T3, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Map(c => selector(a, b, c))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, T4, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, IParser<TToken, T4> parser4, Func<T1, T2, T3, T4, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Bind(c => parser4.Map(d => selector(a, b, c, d)))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, T4, T5, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, IParser<TToken, T4> parser4, IParser<TToken, T5> parser5, Func<T1, T2, T3, T4, T5, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Bind(c => parser4.Bind(d => parser5.Map(e => selector(a, b, c, d, e))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, T4, T5, T6, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, IParser<TToken, T4> parser4, IParser<TToken, T5> parser5, IParser<TToken, T6> parser6, Func<T1, T2, T3, T4, T5, T6, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Bind(c => parser4.Bind(d => parser5.Bind(e => parser6.Map(f => selector(a, b, c, d, e, f)))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, T4, T5, T6, T7, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, IParser<TToken, T4> parser4, IParser<TToken, T5> parser5, IParser<TToken, T6> parser6, IParser<TToken, T7> parser7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Bind(c => parser4.Bind(d => parser5.Bind(e => parser6.Bind(f => parser7.Map(g => selector(a, b, c, d, e, f, g))))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Sequence<TToken, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IParser<TToken, T1> parser1, IParser<TToken, T2> parser2, IParser<TToken, T3> parser3, IParser<TToken, T4> parser4, IParser<TToken, T5> parser5, IParser<TToken, T6> parser6, IParser<TToken, T7> parser7, IParser<TToken, T8> parser8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> selector)
        => parser1.Bind(a => parser2.Bind(b => parser3.Bind(c => parser4.Bind(d => parser5.Bind(e => parser6.Bind(f => parser7.Bind(g => parser8.Map(h => selector(a, b, c, d, e, f, g, h)))))))));

    #endregion

    #region Repetition Control Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Many<TToken, T>(IParser<TToken, T> parser)
        => ManyRec(parser, []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Many1<TToken, T>(IParser<TToken, T> parser)
        => parser.Bind(x => ManyRec(parser, [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> SkipMany<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
        => Fix<TToken, Unit>(self => parser.Next(self.Const, Unit.Instance));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> SkipMany1<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
        => parser.Right(SkipMany(parser));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> ManyTill<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator)
        => ManyTillRec(parser, terminator, []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Many1Till<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator)
        => parser.Bind(x => ManyTillRec(parser, terminator, [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> SkipTill<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, IParser<TToken, T> terminator)
        => Fix<TToken, T>(self => terminator.Alternative(parser.Right(self)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Skip1Till<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, IParser<TToken, T> terminator)
        => parser.Right(SkipTill(parser, terminator));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> TakeTill<TToken, TIgnore>(IParser<TToken, TIgnore> terminator)
        => ManyTill(Any<TToken>(), terminator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> Take1Till<TToken, TIgnore>(IParser<TToken, TIgnore> terminator)
        => Many1Till(Any<TToken>(), terminator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Match<TToken, T>(IParser<TToken, T> parser)
        => SkipTill(Any<TToken>(), parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> Quote<TToken, TIgnore>(IParser<TToken, TIgnore> quote)
        => Quote(quote, quote);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> Quote<TToken, TOpen, TClose>(IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => Any<TToken>().QuotedBy(open, close);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> Quote1<TToken, TIgnore>(IParser<TToken, TIgnore> quote)
        => Quote1(quote, quote);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<TToken>> Quote1<TToken, TOpen, TClose>(IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => Any<TToken>().QuotedBy1(open, close);

    #endregion

    #region Parser LookAhead Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Not<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
        => Not(parser, Unit.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Not<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, T result)
        => new Not<TToken, TIgnore, T>(parser, result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> LookAhead<TToken, T>(IParser<TToken, T> parser)
        => new LookAhead<TToken, T>(parser);

    #endregion

    #region Parser System Helper Combinators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Atom<TToken, T>(IParser<TToken, T> parser)
        => parser.Alternative(failure => Fail<TToken, T>($"At {nameof(Atom)} -> {failure.ToString()}"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Delay<TToken, T>(Func<IParser<TToken, T>> parser)
        => new Delay<TToken, T>(parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Fix<TToken, T>(Func<IParser<TToken, T>, IParser<TToken, T>> function)
        => new Fix<TToken, T>(function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<TParameter, IParser<TToken, T>> Fix<TToken, TParameter, T>(Func<Func<TParameter, IParser<TToken, T>>, TParameter, IParser<TToken, T>> function)
        => parameter => new Fix<TToken, TParameter, T>(function, parameter);

    #endregion

    #region Parser Composition Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TLeft> Left<TToken, TLeft, TRight>(this IParser<TToken, TLeft> left, IParser<TToken, TRight> right)
        => left.Bind(right.MapConst);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TRight> Right<TToken, TLeft, TRight>(this IParser<TToken, TLeft> left, IParser<TToken, TRight> right)
        => left.Bind(right.Const);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Between<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> bracket)
        => parser.Between(bracket, bracket);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Between<TToken, T, TOpen, TClose>(this IParser<TToken, T> parser, IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => open.Right(parser.Left(close));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> QuotedBy<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> quotation)
        => parser.QuotedBy(quotation, quotation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> QuotedBy<TToken, T, TOpen, TClose>(this IParser<TToken, T> parser, IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => open.Right(ManyTill(parser, close));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> QuotedBy1<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> quotation)
        => parser.QuotedBy1(quotation, quotation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> QuotedBy1<TToken, T, TOpen, TClose>(this IParser<TToken, T> parser, IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => open.Right(Many1Till(parser, close));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Or<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second)
        => first.Alternative(second);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> exclusion)
        => Not(exclusion).Right(parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, params IEnumerable<IParser<TToken, TIgnore>> exclusions)
        => parser.Except(Choice(exclusions));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, params IParser<TToken, TIgnore>[] exclusions)
        => parser.Except(exclusions.AsEnumerable());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2)
        => Not(exclusion1).Right(Not(exclusion2).Right(parser));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(parser)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3, IParser<TToken, TIgnore4> exclusion4)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(Not(exclusion4).Right(parser))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3, IParser<TToken, TIgnore4> exclusion4, IParser<TToken, TIgnore5> exclusion5)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(Not(exclusion4).Right(Not(exclusion5).Right(parser)))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3, IParser<TToken, TIgnore4> exclusion4, IParser<TToken, TIgnore5> exclusion5, IParser<TToken, TIgnore6> exclusion6)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(Not(exclusion4).Right(Not(exclusion5).Right(Not(exclusion6).Right(parser))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6, TIgnore7>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3, IParser<TToken, TIgnore4> exclusion4, IParser<TToken, TIgnore5> exclusion5, IParser<TToken, TIgnore6> exclusion6, IParser<TToken, TIgnore7> exclusion7)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(Not(exclusion4).Right(Not(exclusion5).Right(Not(exclusion6).Right(Not(exclusion7).Right(parser)))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6, TIgnore7, TIgnore8>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exclusion1, IParser<TToken, TIgnore2> exclusion2, IParser<TToken, TIgnore3> exclusion3, IParser<TToken, TIgnore4> exclusion4, IParser<TToken, TIgnore5> exclusion5, IParser<TToken, TIgnore6> exclusion6, IParser<TToken, TIgnore7> exclusion7, IParser<TToken, TIgnore8> exclusion8)
        => Not(exclusion1).Right(Not(exclusion2).Right(Not(exclusion3).Right(Not(exclusion4).Right(Not(exclusion5).Right(Not(exclusion6).Right(Not(exclusion7).Right(Not(exclusion8).Right(parser))))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(y => (IReadOnlyList<T>)[x, y]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, IReadOnlyCollection<T>> right)
        => left.Bind(x => right.Map(y => y.Prepend(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(x.Append));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, IReadOnlyCollection<T>> right)
        => left.Bind(x => right.Map(x.Concat));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, IEnumerable<T>> right)
        => left.Bind(x => right.Map(y => y.Prepend(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, IEnumerable<T>> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(x.Append));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, IEnumerable<T>> left, IParser<TToken, IEnumerable<T>> right)
        => left.Bind(x => right.Map(x.Concat));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> Append<TToken>(this IParser<TToken, char> left, IParser<TToken, char> right)
        => left.Bind(x => right.Map(y => x.ToString() + y.ToString()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> Append<TToken>(this IParser<TToken, char> left, IParser<TToken, string> right)
        => left.Bind(x => right.Map(y => x + y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> Append<TToken>(this IParser<TToken, string> left, IParser<TToken, char> right)
        => left.Bind(x => right.Map(y => x + y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, string> Append<TToken>(this IParser<TToken, string> left, IParser<TToken, string> right)
        => left.Bind(x => right.Map(y => x + y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Either(y => (IReadOnlyList<T>)[x, y], [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, IReadOnlyCollection<T>> optional)
        => parser.Bind(x => optional.Either(y => y.Prepend(x), [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Either(x.Append, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> parser, IParser<TToken, IReadOnlyCollection<T>> optional)
        => parser.Bind(x => optional.Either(x.Concat, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, IEnumerable<T>> optional)
        => parser.Bind(x => optional.Either(y => y.Prepend(x), [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, IEnumerable<T>> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Either(x.Append, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, IEnumerable<T>> parser, IParser<TToken, IEnumerable<T>> optional)
        => parser.Bind(x => optional.Either(x.Concat, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> AppendOptional<TToken>(this IParser<TToken, char> parser, IParser<TToken, char> optional)
        => parser.Bind(x => optional.Either(y => x.ToString() + y.ToString(), x.ToString()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> AppendOptional<TToken>(this IParser<TToken, char> parser, IParser<TToken, string> optional)
        => parser.Bind(x => optional.Either(y => x + y, x.ToString()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> AppendOptional<TToken>(this IParser<TToken, string> parser, IParser<TToken, char> optional)
        => parser.Bind(x => optional.Either(y => x + y, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, string> AppendOptional<TToken>(this IParser<TToken, string> parser, IParser<TToken, string> optional)
        => parser.Bind(x => optional.Either(y => x + y, x));

    #endregion

    #region Repetition Separator Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> SeparatedBy<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => Try(parser.SeparatedBy1(separator), []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> SeparatedBy1<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => parser.Bind(x => ManyRec(separator.Right(parser), [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> EndBy<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => Many(parser.Left(separator));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> EndBy1<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => Many1(parser.Left(separator));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> SeparatedOrEndBy<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => Try(parser.SeparatedOrEndBy1(separator), []);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> SeparatedOrEndBy1<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> separator)
        => parser.SeparatedBy1(separator).Left(Optional(separator));

    #endregion

    #region Repetition Control Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int count)
        => Sequence(Enumerable.Repeat(parser, count));

    #endregion

    #region Iterative Application Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Chain<TToken, T>(this IParser<TToken, T> parser, Func<T, IParser<TToken, T>> chain)
        => parser.Bind(x => ChainRec(chain, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> ChainLeft<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, Func<T, T, T>> function)
        => parser.Chain(x => function.Bind(function => parser.Map(y => function(x, y))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> ChainRight<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, Func<T, T, T>> function)
        => Fix<TToken, T>(self => parser.Bind(x => Try(function.Bind(function => self.Map(y => function(x, y))), x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> FoldLeft<TToken, T>(this IParser<TToken, T> parser, Func<T, T, T> function)
        => parser.Bind(x => parser.FoldLeft(x, function));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldLeft<TToken, T, TAccumulator>(this IParser<TToken, T> parser, TAccumulator seed, Func<TAccumulator, T, TAccumulator> function)
        => parser.Next(x => parser.FoldLeft(function(seed, x), function), seed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldLeft<TToken, T, TAccumulator>(this IParser<TToken, T> parser, Func<IParsecState<TToken>, TAccumulator> seed, Func<TAccumulator, T, TAccumulator> function)
        => Pure(seed).Bind(seed => parser.FoldLeft(seed, function));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> FoldRight<TToken, T>(this IParser<TToken, T> parser, Func<T, T, T> function)
        => Fix<TToken, T>(self => parser.Bind(x => self.Either(accumulator => function(x, accumulator), x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldRight<TToken, T, TAccumulator>(this IParser<TToken, T> parser, TAccumulator seed, Func<T, TAccumulator, TAccumulator> function)
        => Fix<TToken, TAccumulator>(self => parser.Next(x => self.Map(accumulator => function(x, accumulator)), seed));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldRight<TToken, T, TAccumulator>(this IParser<TToken, T> parser, Func<IParsecState<TToken>, TAccumulator> seed, Func<T, TAccumulator, TAccumulator> function)
        => Pure(seed).Bind(seed => parser.FoldRight(seed, function));

    #endregion

    #region Result Transformation Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Ignore<TToken, TIgnore>(this IParser<TToken, TIgnore> parser)
        => parser.Map(_ => Unit.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> MapToString<TToken, T>(this IParser<TToken, T> parser)
        => parser.Map(x => x?.ToString() ?? string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T[]> ToArray<TToken, T>(this IParser<TToken, IEnumerable<T>> parser)
        => parser.Map(values => values.ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> AsReadOnlyList<TToken, T>(this IParser<TToken, IReadOnlyList<T>> parser)
        => parser;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> AsReadOnlyCollection<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> parser)
        => parser;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> AsEnumerable<TToken, T>(this IParser<TToken, IEnumerable<T>> parser)
        => parser;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Flatten<TToken, T>(this IParser<TToken, IEnumerable<IReadOnlyCollection<T>>> parser)
        => parser.Map(values => values.Aggregate((IReadOnlyCollection<T>)[], (accumulator, x) => accumulator.Concat(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Flatten<TToken, T>(this IParser<TToken, IEnumerable<IEnumerable<T>>> parser)
        => parser.Map(values => values.SelectMany(x => x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Singleton<TToken, T>(this IParser<TToken, T> parser)
        => parser.Map(x => (IReadOnlyList<T>)[x]);

    #endregion

    #region Text Transformation Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> AsString<TToken>(this IParser<TToken, char> parser)
        => parser.Map(x => x.ToString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> AsString<TToken>(this IParser<TToken, IEnumerable<char>> parser)
        => parser as IParser<TToken, string> ?? parser.Map(values => new string([.. values]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, int> ToInt<TToken>(this IParser<TToken, IEnumerable<char>> parser)
        => parser.AsString().ToInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, int> ToInt<TToken>(this IParser<TToken, string> parser)
        => parser.Bind(value => int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure<TToken, int>(result)
            : Fail<TToken, int>($"Expected digits but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, long> ToLong<TToken>(this IParser<TToken, IEnumerable<char>> parser)
        => parser.AsString().ToLong();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, long> ToLong<TToken>(this IParser<TToken, string> parser)
        => parser.Bind(value => long.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure<TToken, long>(result)
            : Fail<TToken, long>($"Expected digits but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, double> ToDouble<TToken>(this IParser<TToken, IEnumerable<char>> parser)
        => parser.AsString().ToDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, double> ToDouble<TToken>(this IParser<TToken, string> parser)
        => parser.Bind(value => double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result)
            ? Pure<TToken, double>(result)
            : Fail<TToken, double>($"Expected number but was '{value}'"));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> Join<TToken>(this IParser<TToken, IEnumerable<string>> parser)
        => parser.Map(string.Concat);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> Join<TToken>(this IParser<TToken, IEnumerable<string>> parser, string separator)
        => parser.Map(values => string.Join(separator, values));

    #endregion

    #region Parser Validation Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> End<TToken, T>(this IParser<TToken, T> parser)
        => parser.Left(EndOfInput<TToken>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithConsume<TToken, T>(this IParser<TToken, T> parser)
        => parser.WithConsume(_ => "A parser did not consume any input");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithConsume<TToken, T>(this IParser<TToken, T> parser, Func<IPosition, string> message)
        => GetPosition<TToken>().Bind(start => parser.Left(GetPosition<TToken>().Guard(end => !start.Equals(end), message)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithMessage<TToken, T>(this IParser<TToken, T> parser, string message)
        => new OverrideMessage<TToken, T>(parser, message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithMessage<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message)
        => new OverrideMessageDelayed<TToken, T>(parser, message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> AbortWhenFail<TToken, T>(this IParser<TToken, T> parser)
        => parser.AbortWhenFail(failure => $"At {nameof(AbortWhenFail)} -> {failure.ToString()}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> AbortWhenFail<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message)
        => parser.Alternative(failure => Abort<TToken, T>(message(failure).Const));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> AbortIfEntered<TToken, T>(this IParser<TToken, T> parser)
        => parser.AbortIfEntered(failure => $"At {nameof(AbortIfEntered)} -> {failure.ToString()}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> AbortIfEntered<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message)
        => parser.Alternative(failure => GetPosition<TToken>()
            .Bind(position => position.Equals(failure.State.Position) ? Fail<TToken, T>(failure.Message) : Abort<TToken, T>(message(failure).Const)));

    #endregion

    #region Monad Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Bind<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next)
        => new Bind<TToken, T, TResult>(parser, next);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Alternative<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second)
        => new Alternative<TToken, T>(first, second);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Alternative<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, IParser<TToken, T>> resume)
        => new Resume<TToken, T>(parser, resume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Map<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function)
        => new Map<TToken, T, TResult>(parser, function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> MapConst<TToken, TIgnore, T>(this IParser<TToken, TIgnore> parser, T result)
        => parser.Map(_ => result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> MapWithExceptionHandling<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function)
        => new MapWithExceptionHandling<TToken, T, TResult>(parser, function);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Either<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, TResult fallback)
        => new EitherConst<TToken, T, TResult>(parser, function, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Either<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, Func<IFailure<TToken, T>, TResult> fallback)
        => new Either<TToken, T, TResult>(parser, function, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> function, Func<IFailure<TToken, T>, IParser<TToken, TResult>> resume)
        => parser.Next(x => Pure<TToken, TResult>(function(x)), resume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, TResult fallback)
        => parser.Next(next, Pure<TToken, TResult>(fallback).Const);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, Func<IFailure<TToken, T>, TResult> fallback)
        => parser.Next(next, failure => Pure<TToken, TResult>(fallback(failure)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TResult> Next<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TResult>> next, Func<IFailure<TToken, T>, IParser<TToken, TResult>> resume)
        => new Next<TToken, T, TResult>(parser, next, resume);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Flatten<TToken, T>(this IParser<TToken, IParser<TToken, T>> parser)
        => parser.Bind(parser => parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Guard<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate)
        => parser.Guard(predicate, x => $"A value '{x?.ToString() ?? "<null>"}' does not satisfy condition");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Guard<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate, Func<T, string> message)
        => parser.Bind(x => predicate(x) ? Pure<TToken, T>(x) : Fail<TToken, T>(message(x)));

    #endregion

    #region Side Effect Helper Extensions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Do<TToken, T>(this IParser<TToken, T> parser, Action<T> action)
        => parser.Map(x => { action(x); return x; });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Do<TToken, T>(this IParser<TToken, T> parser, Action<T> action, Action<IFailure<TToken, T>> failure)
        => new Do<TToken, T>(parser, failure, action);

    #endregion

    #region Private Implementation Helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IParser<TToken, IReadOnlyCollection<T>> ManyRec<TToken, T>(IParser<TToken, T> parser, IReadOnlyCollection<T> result)
        => parser.Next(x => ManyRec(parser, result.Append(x)), result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IParser<TToken, IReadOnlyCollection<T>> ManyTillRec<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator, IReadOnlyCollection<T> result)
        => terminator.Next(result.Const, parser.Bind(x => ManyTillRec(parser, terminator, result.Append(x))).Const);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IParser<TToken, T> ChainRec<TToken, T>(Func<T, IParser<TToken, T>> chain, T value)
        => chain(value).Next(x => ChainRec(chain, x), value);

    #endregion

    #region LINQ Query Support

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IParser<TToken, TResult> Select<TToken, T, TResult>(this IParser<TToken, T> parser, Func<T, TResult> selector)
        => parser.Map(selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IParser<TToken, TResult> SelectMany<TToken, T, TIntermediate, TResult>(this IParser<TToken, T> parser, Func<T, IParser<TToken, TIntermediate>> selector, Func<T, TIntermediate, TResult> projector)
        => parser.Bind(x => selector(x).Map(y => projector(x, y)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IParser<TToken, T> Where<TToken, T>(this IParser<TToken, T> parser, Func<T, bool> predicate)
        => parser.Guard(predicate);

    #endregion

    #region Operator Overloads

    extension<TToken, T>(IParser<TToken, T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> operator |(IParser<TToken, T> first, IParser<TToken, T> second)
            => first.Alternative(second);
    }

    extension<TToken, T>(IParser<TToken, T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyList<T>> operator +(IParser<TToken, T> left, IParser<TToken, T> right)
            => left.Append(right);
    }

    extension<TToken, T>(IParser<TToken, IReadOnlyCollection<T>>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, T> left, IParser<TToken, IReadOnlyCollection<T>> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, T> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, IReadOnlyCollection<T>> operator +(IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, IReadOnlyCollection<T>> right)
            => left.Append(right);
    }

    extension<TToken, T>(IParser<TToken, IEnumerable<T>>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, T> left, IParser<TToken, IEnumerable<T>> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, IEnumerable<T>> left, IParser<TToken, T> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, IEnumerable<T>> operator +(IParser<TToken, IEnumerable<T>> left, IParser<TToken, IEnumerable<T>> right)
            => left.Append(right);
    }

    extension<TToken>(IParser<TToken, char>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, string> operator +(IParser<TToken, char> left, IParser<TToken, char> right)
            => left.Append(right);
    }

    extension<TToken>(IParser<TToken, string>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, string> operator +(IParser<TToken, char> left, IParser<TToken, string> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, string> operator +(IParser<TToken, string> left, IParser<TToken, char> right)
            => left.Append(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public static IParser<TToken, string> operator +(IParser<TToken, string> left, IParser<TToken, string> right)
            => left.Append(right);
    }

    #endregion
}
