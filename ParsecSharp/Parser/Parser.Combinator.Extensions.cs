using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, IEnumerable<T>> SepBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Try(parser.SepBy1(separator), Enumerable.Empty<T>());

        public static Parser<TToken, IEnumerable<T>> SepBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.Bind(x => Many_(separator.Right(parser), new List<T> { x }));

        public static Parser<TToken, IEnumerable<T>> EndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many(parser.Left(separator));

        public static Parser<TToken, IEnumerable<T>> EndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many1(parser.Left(separator));

        public static Parser<TToken, IEnumerable<T>> SepEndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Try(parser.SepEndBy1(separator), Enumerable.Empty<T>());

        public static Parser<TToken, IEnumerable<T>> SepEndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.SepBy1(separator).Left(Optional(separator));

        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> exception)
            => Not(exception).Right(parser);

        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, IEnumerable<Parser<TToken, TIgnore>> exceptions)
            => parser.Except(Choice(exceptions));

        public static Parser<TToken, T> Except<TToken, T, TIgnore>(this Parser<TToken, T> parser, params Parser<TToken, TIgnore>[] exceptions)
            => parser.Except(exceptions.AsEnumerable());

        public static Parser<TToken, T> Chain<TToken, T>(this Parser<TToken, T> parser, Func<T, Parser<TToken, T>> rest)
            => parser.Bind(x => Chain_(rest, x));

        private static Parser<TToken, T> Chain_<TToken, T>(Func<T, Parser<TToken, T>> rest, T value)
            => Try(rest(value).Bind(x => Chain_(rest, x)), value);

        public static Parser<TToken, TAccum> FoldL<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<TAccum, T, TAccum> function)
            => Try(parser.Bind(x => parser.FoldL(function(seed, x), function)), seed);

        public static Parser<TToken, TAccum> FoldL<TToken, T, TAccum>(this Parser<TToken, T> parser, Func<TAccum> seed, Func<TAccum, T, TAccum> function)
            => Pure<TToken, TAccum>(seed).Bind(x => parser.FoldL(x, function));

        public static Parser<TToken, TAccum> FoldR<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<T, TAccum, TAccum> function)
            => Try(parser.Bind(x => parser.FoldR(seed, function).Map(accum => function(x, accum))), seed);

        public static Parser<TToken, TAccum> FoldR<TToken, T, TAccum>(this Parser<TToken, T> parser, Func<TAccum> seed, Func<T, TAccum, TAccum> function)
            => Pure<TToken, TAccum>(seed).Bind(x => parser.FoldR(x, function));

        public static Parser<TToken, IEnumerable<T>> Repeat<TToken, T>(this Parser<TToken, T> parser, int count)
            => Sequence(Enumerable.Repeat(parser, count));

        public static Parser<TToken, TLeft> Left<TToken, TLeft, TRight>(this Parser<TToken, TLeft> parser, Parser<TToken, TRight> next)
            => parser.Bind(x => next.Map(_ => x));

        public static Parser<TToken, TRight> Right<TToken, TLeft, TRight>(this Parser<TToken, TLeft> parser, Parser<TToken, TRight> next)
            => parser.Bind(_ => next);

        public static Parser<TToken, T> Between<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> bracket)
            => parser.Between(bracket, bracket);

        public static Parser<TToken, T> Between<TToken, T, TOpen, TClose>(this Parser<TToken, T> parser, Parser<TToken, TOpen> open, Parser<TToken, TClose> close)
            => open.Right(parser.Left(close));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => parser.Bind(x => next.Map(y => new[] { x, y }.AsEnumerable()));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, IEnumerable<T>> next)
            => parser.Bind(x => next.Map(y => y.Prepend(x)));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> parser, Parser<TToken, T> next)
            => parser.Bind(x => next.Map(y => x.Append(y)));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> parser, Parser<TToken, IEnumerable<T>> next)
            => parser.Bind(x => next.Map(y => x.Concat(y)));

        public static Parser<TToken, T> Or<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => parser.Alternative(next);

        public static Parser<TToken, Unit> Ignore<TToken, TIgnore>(this Parser<TToken, TIgnore> parser)
            => parser.Map(_ => Unit.Instance);

        public static Parser<TToken, string> ToStr<TToken, T>(this Parser<TToken, T> parser)
            => parser.Map(x => x?.ToString() ?? string.Empty);

        public static Parser<TToken, T[]> ToArray<TToken, T>(this Parser<TToken, IEnumerable<T>> parser)
            => parser.Map(x => x.ToArray());

        public static Parser<TToken, T> Message<TToken, T>(this Parser<TToken, T> parser, Func<IParsecState<TToken>, string> message)
            => parser.Alternative(Fail<TToken, T>(message));

        public static Parser<TToken, T> Error<TToken, T>(this Parser<TToken, T> parser, Func<IParsecState<TToken>, string> message)
            => parser.Alternative(Abort<TToken, T>(message));
    }
}
