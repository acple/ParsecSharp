using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ParsecSharp.Internal.Parsers;

namespace ParsecSharp
{
    public static partial class Parser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Choice<TToken, T>(IEnumerable<IParser<TToken, T>> parsers)
            => parsers.Reverse().Aggregate((next, parser) => parser.Alternative(next));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Choice<TToken, T>(params IParser<TToken, T>[] parsers)
            => Choice(parsers.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> Sequence<TToken, T>(IEnumerable<IParser<TToken, T>> parsers)
            => parsers.Reverse()
                .Aggregate(Pure<TToken, IEnumerable<T>>([]),
                    (next, parser) => parser.Bind(x => next.Map(result => result.Prepend(x))));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> Sequence<TToken, T>(params IParser<TToken, T>[] parsers)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Try<TToken, T>(IParser<TToken, T> parser, T resume)
            => parser.Alternative(Pure<TToken, T>(resume));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Try<TToken, T>(IParser<TToken, T> parser, Func<IFailure<TToken, T>, T> resume)
            => new Try<TToken, T>(parser, resume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Optional<TToken, T>(IParser<TToken, T> parser, T defaultValue)
            => Try(parser, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, bool> Optional<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
            => parser.Next(_ => true, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Not<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, T result)
            => new Not<TToken, TIgnore, T>(parser, result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> Not<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
            => Not(parser, Unit.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> LookAhead<TToken, T>(IParser<TToken, T> parser)
            => new LookAhead<TToken, T>(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> Many<TToken, T>(IParser<TToken, T> parser)
            => ManyRec(parser, []);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> Many1<TToken, T>(IParser<TToken, T> parser)
            => parser.Bind(x => ManyRec(parser, [x]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> SkipMany<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
            => Fix<TToken, Unit>(self => parser.Next(self.Const, Unit.Instance));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, Unit> SkipMany1<TToken, TIgnore>(IParser<TToken, TIgnore> parser)
            => parser.Right(SkipMany(parser));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> ManyTill<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator)
            => ManyTillRec(parser, terminator, []);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<T>> Many1Till<TToken, T, TIgnore>(IParser<TToken, T> parser, IParser<TToken, TIgnore> terminator)
            => parser.Bind(x => ManyTillRec(parser, terminator, [x]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> SkipTill<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, IParser<TToken, T> terminator)
            => Fix<TToken, T>(self => terminator.Alternative(parser.Right(self)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Skip1Till<TToken, TIgnore, T>(IParser<TToken, TIgnore> parser, IParser<TToken, T> terminator)
            => parser.Right(SkipTill(parser, terminator));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> TakeTill<TToken, TIgnore>(IParser<TToken, TIgnore> terminator)
            => ManyTill(Any<TToken>(), terminator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> Take1Till<TToken, TIgnore>(IParser<TToken, TIgnore> terminator)
            => Many1Till(Any<TToken>(), terminator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Match<TToken, T>(IParser<TToken, T> parser)
            => SkipTill(Any<TToken>(), parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> Quoted<TToken, TIgnore>(IParser<TToken, TIgnore> quote)
            => Quoted(quote, quote);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, IEnumerable<TToken>> Quoted<TToken, TOpen, TClose>(IParser<TToken, TOpen> open, IParser<TToken, TClose> close)
            => Any<TToken>().Quote(open, close);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Atom<TToken, T>(IParser<TToken, T> parser)
            => parser.WithMessage(failure => $"At {nameof(Atom)} -> {failure.ToString()}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Delay<TToken, T>(Func<IParser<TToken, T>> parser)
            => new Delay<TToken, T>(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParser<TToken, T> Fix<TToken, T>(Func<IParser<TToken, T>, IParser<TToken, T>> function)
            => new Fix<TToken, T>(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TParameter, IParser<TToken, T>> Fix<TToken, TParameter, T>(Func<Func<TParameter, IParser<TToken, T>>, TParameter, IParser<TToken, T>> function)
            => parameter => new Fix<TToken, TParameter, T>(function, parameter);
    }
}
