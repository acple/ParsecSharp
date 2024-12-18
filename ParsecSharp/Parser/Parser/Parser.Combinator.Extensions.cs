using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParsecSharp;

public static partial class Parser
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> exception)
        => Not(exception).Right(parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, params IEnumerable<IParser<TToken, TIgnore>> exceptions)
        => parser.Except(Choice(exceptions));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore>(this IParser<TToken, T> parser, params IParser<TToken, TIgnore>[] exceptions)
        => parser.Except(exceptions.AsEnumerable());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2)
        => Not(exception1).Right(Not(exception2).Right(parser));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(parser)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3, IParser<TToken, TIgnore4> exception4)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(Not(exception4).Right(parser))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3, IParser<TToken, TIgnore4> exception4, IParser<TToken, TIgnore5> exception5)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(Not(exception4).Right(Not(exception5).Right(parser)))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3, IParser<TToken, TIgnore4> exception4, IParser<TToken, TIgnore5> exception5, IParser<TToken, TIgnore6> exception6)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(Not(exception4).Right(Not(exception5).Right(Not(exception6).Right(parser))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6, TIgnore7>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3, IParser<TToken, TIgnore4> exception4, IParser<TToken, TIgnore5> exception5, IParser<TToken, TIgnore6> exception6, IParser<TToken, TIgnore7> exception7)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(Not(exception4).Right(Not(exception5).Right(Not(exception6).Right(Not(exception7).Right(parser)))))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Except<TToken, T, TIgnore1, TIgnore2, TIgnore3, TIgnore4, TIgnore5, TIgnore6, TIgnore7, TIgnore8>(this IParser<TToken, T> parser, IParser<TToken, TIgnore1> exception1, IParser<TToken, TIgnore2> exception2, IParser<TToken, TIgnore3> exception3, IParser<TToken, TIgnore4> exception4, IParser<TToken, TIgnore5> exception5, IParser<TToken, TIgnore6> exception6, IParser<TToken, TIgnore7> exception7, IParser<TToken, TIgnore8> exception8)
        => Not(exception1).Right(Not(exception2).Right(Not(exception3).Right(Not(exception4).Right(Not(exception5).Right(Not(exception6).Right(Not(exception7).Right(Not(exception8).Right(parser))))))));

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
        => Fix<TToken, T>(self => parser.Bind(x => self.Next(accumulator => function(x, accumulator), x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldRight<TToken, T, TAccumulator>(this IParser<TToken, T> parser, TAccumulator seed, Func<T, TAccumulator, TAccumulator> function)
        => Fix<TToken, TAccumulator>(self => parser.Next(x => self.Map(accumulator => function(x, accumulator)), seed));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, TAccumulator> FoldRight<TToken, T, TAccumulator>(this IParser<TToken, T> parser, Func<IParsecState<TToken>, TAccumulator> seed, Func<T, TAccumulator, TAccumulator> function)
        => Pure(seed).Bind(seed => parser.FoldRight(seed, function));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Repeat<TToken, T>(this IParser<TToken, T> parser, int count)
        => Sequence(Enumerable.Repeat(parser, count));

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
    public static IParser<TToken, IReadOnlyCollection<T>> Quote<TToken, T, TIgnore>(this IParser<TToken, T> parser, IParser<TToken, TIgnore> quote)
        => parser.Quote(quote, quote);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Quote<TToken, T, TOpen, TClose>(this IParser<TToken, T> parser, IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
        => open.Right(ManyTill(parser, close));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(y => (IReadOnlyList<T>)[x, y]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, IEnumerable<T>> right)
        => left.Bind(x => right.Map(y => y.Prepend(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, T> left, IParser<TToken, IReadOnlyCollection<T>> right)
        => left.Bind(x => right.Map(y => y.Prepend(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, IEnumerable<T>> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(y => x.Append(y)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, T> right)
        => left.Bind(x => right.Map(y => x.Append(y)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IEnumerable<T>> Append<TToken, T>(this IParser<TToken, IEnumerable<T>> left, IParser<TToken, IEnumerable<T>> right)
        => left.Bind(x => right.Map(y => x.Concat(y)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IReadOnlyCollection<T>> Append<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> left, IParser<TToken, IReadOnlyCollection<T>> right)
        => left.Bind(x => right.Map(y => x.Concat(y)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, string> Append<TToken>(this IParser<TToken, string> left, IParser<TToken, string> right)
        => left.Bind(x => right.Map(y => x + y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Next(y => (IReadOnlyList<T>)[x, y], [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, IEnumerable<T>> optional)
        => parser.Bind(x => optional.Next(y => y.Prepend(x), [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, T> parser, IParser<TToken, IReadOnlyCollection<T>> optional)
        => parser.Bind(x => optional.Next(y => y.Prepend(x), [x]));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, IEnumerable<T>> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Next(y => x.Append(y), x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> parser, IParser<TToken, T> optional)
        => parser.Bind(x => optional.Next(y => x.Append(y), x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IEnumerable<T>> AppendOptional<TToken, T>(this IParser<TToken, IEnumerable<T>> parser, IParser<TToken, IEnumerable<T>> optional)
        => parser.Bind(x => optional.Next(y => x.Concat(y), x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, IReadOnlyCollection<T>> AppendOptional<TToken, T>(this IParser<TToken, IReadOnlyCollection<T>> parser, IParser<TToken, IReadOnlyCollection<T>> optional)
        => parser.Bind(x => optional.Next(y => x.Concat(y), x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static IParser<TToken, string> AppendOptional<TToken>(this IParser<TToken, string> parser, IParser<TToken, string> optional)
        => parser.Bind(x => optional.Next(y => x + y, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> Or<TToken, T>(this IParser<TToken, T> first, IParser<TToken, T> second)
        => first.Alternative(second);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, Unit> Ignore<TToken, TIgnore>(this IParser<TToken, TIgnore> parser)
        => parser.Map(_ => Unit.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> End<TToken, T>(this IParser<TToken, T> parser)
        => parser.Left(EndOfInput<TToken>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, string> MapToString<TToken, T>(this IParser<TToken, T> parser)
        => parser.Map(x => x?.ToString() ?? string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T[]> ToArray<TToken, T>(this IParser<TToken, IEnumerable<T>> parser)
        => parser.Map(values => values.ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IEnumerable<T>> Flatten<TToken, T>(this IParser<TToken, IEnumerable<IEnumerable<T>>> parser)
        => parser.Map(values => values.SelectMany(x => x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyCollection<T>> Flatten<TToken, T>(this IParser<TToken, IEnumerable<IReadOnlyCollection<T>>> parser)
        => parser.Map(values => values.Aggregate((IReadOnlyCollection<T>)[], (accumulator, x) => accumulator.Concat(x)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, IReadOnlyList<T>> Singleton<TToken, T>(this IParser<TToken, T> parser)
        => parser.Map(x => (IReadOnlyList<T>)[x]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithConsume<TToken, T>(this IParser<TToken, T> parser)
        => parser.WithConsume(_ => "A parser did not consume any input");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithConsume<TToken, T>(this IParser<TToken, T> parser, Func<IPosition, string> message)
        => GetPosition<TToken>().Bind(start => parser.Left(GetPosition<TToken>().Guard(end => !start.Equals(end), message)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithMessage<TToken, T>(this IParser<TToken, T> parser, string message)
        => parser.Alternative(Fail<TToken, T>(message));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParser<TToken, T> WithMessage<TToken, T>(this IParser<TToken, T> parser, Func<IFailure<TToken, T>, string> message)
        => parser.Alternative(failure => Fail<TToken, T>(message(failure)));

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
}
