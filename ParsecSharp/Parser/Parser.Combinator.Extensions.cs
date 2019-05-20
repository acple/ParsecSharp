using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next, TResult result)
            => parser.Bind(next, _ => Pure<TToken, TResult>(result));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, Parser<TToken, TResult>> next, Func<TResult> result)
            => parser.Bind(next, _ => Pure<TToken, TResult>(result));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TResult> Next<TToken, T, TResult>(this Parser<TToken, T> parser, Func<T, TResult> result, Func<Fail<TToken, T>, Parser<TToken, TResult>> resume)
            => parser.Bind(x => Pure<TToken, TResult>(result(x)), resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> SepBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Try(parser.SepBy1(separator), Enumerable.Empty<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> SepBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.Bind(x => ManyRec(separator.Right(parser), new List<T> { x }));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> EndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many(parser.Left(separator));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> EndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many1(parser.Left(separator));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> SepEndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Try(parser.SepEndBy1(separator), Enumerable.Empty<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> SepEndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.SepBy1(separator).Left(Optional(separator));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> exception)
            => Not(exception).Right(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, IEnumerable<Parser<TToken, TIgnore>> exceptions)
            => parser.Except(Choice(exceptions));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, params Parser<TToken, TIgnore>[] exceptions)
            => parser.Except(exceptions.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Chain<TToken, T>(this Parser<TToken, T> parser, Func<T, Parser<TToken, T>> rest)
            => parser.Bind(x => ChainRec(rest, x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> ChainL<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function)
            => parser.Chain(x => function.Bind(func => parser.Map(y => func(x, y))));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> ChainR<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function)
            => Fix<TToken, T>(self => parser.Bind(x => Try(function.Bind(func => self.Map(y => func(x, y))), x)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TAccum> FoldL<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<TAccum, T, TAccum> function)
            => parser.Next(x => parser.FoldL(function(seed, x), function), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TAccum> FoldL<TToken, T, TAccum>(this Parser<TToken, T> parser, Func<TAccum> seed, Func<TAccum, T, TAccum> function)
            => Pure<TToken, TAccum>(seed).Bind(x => parser.FoldL(x, function));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TAccum> FoldR<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<T, TAccum, TAccum> function)
            => Fix<TToken, TAccum>(self => parser.Next(x => self.Map(accum => function(x, accum)), seed));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TAccum> FoldR<TToken, T, TAccum>(this Parser<TToken, T> parser, Func<TAccum> seed, Func<T, TAccum, TAccum> function)
            => Pure<TToken, TAccum>(seed).Bind(x => parser.FoldR(x, function));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Repeat<TToken, T>(this Parser<TToken, T> parser, int count)
            => Sequence(Enumerable.Repeat(parser, count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TLeft> Left<TToken, TLeft, TRight>(this Parser<TToken, TLeft> left, Parser<TToken, TRight> right)
            => left.Bind(x => right.Map(_ => x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, TRight> Right<TToken, TLeft, TRight>(this Parser<TToken, TLeft> left, Parser<TToken, TRight> right)
            => left.Bind(_ => right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Between<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> bracket)
            => parser.Between(bracket, bracket);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Between<TToken, T, TOpen, TClose>(this Parser<TToken, T> parser, Parser<TToken, TOpen> open, Parser<TToken, TClose> close)
            => open.Right(parser.Left(close));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> left, Parser<TToken, T> right)
            => left.Bind(x => right.Map(y => new[] { x, y }.AsEnumerable()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> left, Parser<TToken, IEnumerable<T>> right)
            => left.Bind(x => right.Map(y => y.Prepend(x)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> left, Parser<TToken, T> right)
            => left.Bind(x => right.Map(y => x.Append(y)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> left, Parser<TToken, IEnumerable<T>> right)
            => left.Bind(x => right.Map(y => x.Concat(y)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Or<TToken, T>(this Parser<TToken, T> first, Parser<TToken, T> second)
            => first.Alternative(second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> Ignore<TToken, TIgnore>(this Parser<TToken, TIgnore> parser)
            => parser.Map(_ => Unit.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> End<TToken, T>(this Parser<TToken, T> parser)
            => parser.Left(EndOfInput<TToken>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, string> ToStr<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x?.ToString() ?? string.Empty);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T[]> ToArray<TToken, T>(this Parser<TToken, IEnumerable<T>> parser)
            => parser.Map(x => x.ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> WithMessage<TToken, T>(this Parser<TToken, T> parser, string message)
            => parser.Alternative(Fail<TToken, T>(message));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> WithMessage<TToken, T>(this Parser<TToken, T> parser, Func<Fail<TToken, T>, string> message)
            => parser.Alternative(fail => Fail<TToken, T>(message(fail)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Error<TToken, T>(this Parser<TToken, T> parser, Func<Fail<TToken, T>, string> message)
            => parser.Alternative(fail => Abort<TToken, T>(_ => message(fail)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> AbortIfEntered<TToken, T>(this Parser<TToken, T> parser, Func<Fail<TToken, T>, string> message)
            => parser.Alternative(fail => GetPosition<TToken>()
                .Bind(position => (position.Equals(fail.State.Position)) ? Fail<TToken, T>(fail.Message) : Abort<TToken, T>(_ => message(fail))));
    }
}
