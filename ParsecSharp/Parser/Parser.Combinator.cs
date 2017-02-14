using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Internal;

namespace Parsec
{
    public static partial class Parser
    {
        public static Parser<TToken, T> Choice<TToken, T>(IEnumerable<Parser<TToken, T>> parsers)
            => parsers.Aggregate((parser, next) => parser.Alternative(next));

        public static Parser<TToken, T> Choice<TToken, T>(params Parser<TToken, T>[] parsers)
            => Choice(parsers.AsEnumerable());

        public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(IEnumerable<Parser<TToken, T>> parsers)
            => parsers
                .Aggregate(Return<TToken, List<T>>(() => new List<T>()),
                    (parser, next) => parser.Bind(list => next.FMap(x => { list.Add(x); return list; })))
                .FMap(list => list.AsEnumerable());

        public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(params Parser<TToken, T>[] parsers)
            => Sequence(parsers.AsEnumerable());

        public static Parser<TToken, T> Try<TToken, T>(Parser<TToken, T> parser, Func<T> resume)
            => parser.Alternative(Return<TToken, T>(resume));

        public static Parser<TToken, T> Optional<TToken, T>(Parser<TToken, T> parser, T value)
            => Try(parser, () => value);

        public static Parser<TToken, Unit> Optional<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => Try(parser.Ignore(), () => Unit.Instance);

        public static Parser<TToken, Unit> Not<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => parser.ModifyResult(
                (state, _) => Result.Success(Unit.Instance, state),
                (state, _) => Result.Fail<TToken, Unit>(state));

        public static Parser<TToken, T> LookAhead<TToken, T>(Parser<TToken, T> parser)
            => parser.ModifyResult(
                (_, fail) => fail,
                (state, success) => Result.Success(success.Value, state));

        public static Parser<TToken, IEnumerable<T>> Many<TToken, T>(Parser<TToken, T> parser)
            => Try(Many1(parser), () => Enumerable.Empty<T>());

        public static Parser<TToken, IEnumerable<T>> Many1<TToken, T>(Parser<TToken, T> parser)
            => parser.Bind(x => Many_(parser, new List<T> { x }));

        private static Parser<TToken, IEnumerable<T>> Many_<TToken, T>(Parser<TToken, T> parser, List<T> list)
            => Try(parser.Bind(x => { list.Add(x); return Many_(parser, list); }), () => list);

        public static Parser<TToken, IEnumerable<T>> ManyTill<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator)
            => Return<TToken, List<T>>(() => new List<T>()).Bind(list => ManyTill_(parser, terminator, list));

        private static Parser<TToken, IEnumerable<T>> ManyTill_<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator, List<T> list)
            => terminator.FMap(_ => list.AsEnumerable())
                .Alternative(parser.Bind(x => { list.Add(x); return ManyTill_(parser, terminator, list); }));

        public static Parser<TToken, Unit> SkipMany<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => Try(parser.Bind(_ => SkipMany(parser)), () => Unit.Instance);

        public static Parser<TToken, Unit> SkipMany1<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => parser.Right(SkipMany(parser));

        public static Parser<TToken, T> Delay<TToken, T>(Func<Parser<TToken, T>> parser)
            => new Delay<TToken, T>(parser);

        public static Parser<TToken, T> Fix<TToken, T>(Func<Parser<TToken, T>, Parser<TToken, T>> parser)
            => new Fix<TToken, T>(parser);
    }
}
