using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, IEnumerable<T>> SepBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Try(parser.SepBy1(separator), () => Enumerable.Empty<T>());

        public static Parser<TToken, IEnumerable<T>> SepBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.Bind(x => Many_(separator.Right(parser), new List<T>() { x }));

        public static Parser<TToken, IEnumerable<T>> EndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many(parser.Left(separator));

        public static Parser<TToken, IEnumerable<T>> EndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => Many1(parser.Left(separator));

        public static Parser<TToken, IEnumerable<T>> SepEndBy<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.SepBy(separator).Left(Optional(separator));

        public static Parser<TToken, IEnumerable<T>> SepEndBy1<TToken, T, TIgnore>(this Parser<TToken, T> parser, Parser<TToken, TIgnore> separator)
            => parser.SepBy1(separator).Left(Optional(separator));

        public static Parser<TToken, T> ChainL<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function)
            => parser.Bind(x => parser.ChainL_(function, x));

        private static Parser<TToken, T> ChainL_<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function, T accum)
            => function.Bind(func => parser.FMap(x => func(accum, x)).Bind(y => parser.ChainL_(function, y)),
                () => Return<TToken, T>(accum));

        public static Parser<TToken, T> ChainR<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function)
            => parser.ChainR_(function, result => result);

        private static Parser<TToken, T> ChainR_<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, Func<T, T, T>> function, Func<T, T> cont)
            => parser.Bind(x => function.Bind(func => parser.ChainR_(function, result => cont(func(x, result))),
                () => Return<TToken, T>(cont(x))));

        public static Parser<TToken, TAccum> FoldL<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<TAccum, T, TAccum> accumulator)
            => parser.Bind(x => parser.FoldL(accumulator(seed, x), accumulator), () => Return<TToken, TAccum>(seed));

        public static Parser<TToken, TAccum> FoldR<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<T, TAccum, TAccum> accumulator)
            => parser.FoldR_(seed, accumulator, result => result);

        private static Parser<TToken, TAccum> FoldR_<TToken, T, TAccum>(this Parser<TToken, T> parser, TAccum seed, Func<T, TAccum, TAccum> accumulator, Func<TAccum, TAccum> cont)
            => parser.Bind(x => parser.FoldR_(seed, accumulator, result => cont(accumulator(x, result))),
                () => Return<TToken, TAccum>(cont(seed)));

        public static Parser<TToken, IEnumerable<T>> Repeat<TToken, T>(this Parser<TToken, T> parser, int count)
            => Sequence(Enumerable.Repeat(parser, count));

        public static Parser<TToken, TLeft> Left<TToken, TLeft, TRight>(this Parser<TToken, TLeft> parser, Parser<TToken, TRight> next)
            => parser.Bind(x => next.FMap(_ => x));

        public static Parser<TToken, TRight> Right<TToken, TLeft, TRight>(this Parser<TToken, TLeft> parser, Parser<TToken, TRight> next)
            => parser.Bind(_ => next);

        public static Parser<TToken, T> Between<TToken, T, TLeft, TRight>(this Parser<TToken, T> parser, Parser<TToken, TLeft> left, Parser<TToken, TRight> right)
            => left.Right(parser.Left(right));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => parser.Bind(x => next.FMap(y => new[] { x, y }.AsEnumerable()));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, IEnumerable<T>> next)
            => parser.Bind(x => next.FMap(y => new[] { x }.Concat(y)));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> parser, Parser<TToken, T> next)
            => parser.Bind(x => next.FMap(y => x.Concat(new[] { y })));

        public static Parser<TToken, IEnumerable<T>> Append<TToken, T>(this Parser<TToken, IEnumerable<T>> parser, Parser<TToken, IEnumerable<T>> next)
            => parser.Bind(x => next.FMap(y => x.Concat(y)));

        public static Parser<TToken, T> Or<TToken, T>(this Parser<TToken, T> parser, Parser<TToken, T> next)
            => parser.Alternative(next);

        public static Parser<TToken, Unit> Ignore<TToken, T>(this Parser<TToken, T> parser)
            => parser.FMap(_ => Unit.Instance);

        public static Parser<TToken, T> OnError<TToken, T>(this Parser<TToken, T> parser, Func<IParsecStateStream<TToken>, string> message)
            => parser.ModifyResult(
                (state, fail) => Result.Fail<TToken, T>(message(state), state),
                (_, success) => success);
    }
}
