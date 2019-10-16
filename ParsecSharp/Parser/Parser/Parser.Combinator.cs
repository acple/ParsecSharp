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
        public static Parser<TToken, T> Choice<TToken, T>(IEnumerable<Parser<TToken, T>> parsers)
            => parsers.Reverse().Aggregate((next, parser) => parser.Alternative(next));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Choice<TToken, T>(params Parser<TToken, T>[] parsers)
            => Choice(parsers.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(IEnumerable<Parser<TToken, T>> parsers)
            => parsers.Reverse()
                .Aggregate(Pure<TToken, Stack<T>>(() => new Stack<T>()),
                    (next, parser) => parser.Bind(x => next.Map(stack => { stack.Push(x); return stack; })))
                .Map(stack => stack.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(params Parser<TToken, T>[] parsers)
            => Sequence(parsers.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Try<TToken, T>(Parser<TToken, T> parser, T resume)
            => parser.Alternative(Pure<TToken, T>(resume));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Try<TToken, T>(Parser<TToken, T> parser, Func<T> resume)
            => parser.Alternative(Pure<TToken, T>(resume));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Optional<TToken, T>(Parser<TToken, T> parser, T defaultValue)
            => Try(parser, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, bool> Optional<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => parser.Next(_ => true, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> Not<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => parser.ModifyResult(
                (state, _) => Result.Success(Unit.Instance, state),
                (state, success) => Result.Fail<TToken, Unit>($"At {nameof(Not)} -> Unexpected succeed '{success.ToString()}'", state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> LookAhead<TToken, T>(Parser<TToken, T> parser)
            => parser.ModifyResult(
                (state, fail) => Result.Fail<TToken, T>($"At {nameof(LookAhead)} -> {fail.ToString()}", state),
                (state, success) => Result.Success(success.Value, state));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Many<TToken, T>(Parser<TToken, T> parser)
            => Try(Many1(parser), Enumerable.Empty<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Many1<TToken, T>(Parser<TToken, T> parser)
            => parser.Bind(x => ManyRec(parser, new List<T> { x }));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> SkipMany<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => Fix<TToken, Unit>(self => parser.Next(_ => self, Unit.Instance));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, Unit> SkipMany1<TToken, TIgnore>(Parser<TToken, TIgnore> parser)
            => parser.Right(SkipMany(parser));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> ManyTill<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator)
            => Pure<TToken, List<T>>(() => new List<T>()).Bind(list => ManyTillRec(parser, terminator, list));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Many1Till<TToken, T, TIgnore>(Parser<TToken, T> parser, Parser<TToken, TIgnore> terminator)
            => parser.Bind(x => ManyTillRec(parser, terminator, new List<T> { x }));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> SkipTill<TToken, TIgnore, T>(Parser<TToken, TIgnore> parser, Parser<TToken, T> terminator)
            => Fix<TToken, T>(self => terminator.Alternative(parser.Right(self)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Skip1Till<TToken, TIgnore, T>(Parser<TToken, TIgnore> parser, Parser<TToken, T> terminator)
            => parser.Right(SkipTill(parser, terminator));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> TakeTill<TToken, TIgnore>(Parser<TToken, TIgnore> terminator)
            => ManyTill(Any<TToken>(), terminator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> Take1Till<TToken, TIgnore>(Parser<TToken, TIgnore> terminator)
            => Many1Till(Any<TToken>(), terminator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Match<TToken, T>(Parser<TToken, T> parser)
            => SkipTill(Any<TToken>(), parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> Quoted<TToken, TIgnore>(Parser<TToken, TIgnore> quote)
            => Quoted(quote, quote);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<TToken>> Quoted<TToken, TOpen, TClose>(Parser<TToken, TOpen> open, Parser<TToken, TClose> close)
            => Any<TToken>().Quote(open, close);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, IEnumerable<T>> Singleton<TToken, T>(Parser<TToken, T> parser)
            => parser.Map(x => new[] { x }.AsEnumerable());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Delay<TToken, T>(Func<Parser<TToken, T>> parser)
            => new Delay<TToken, T>(parser);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parser<TToken, T> Fix<TToken, T>(Func<Parser<TToken, T>, Parser<TToken, T>> function)
            => new Fix<TToken, T>(function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TParam, Parser<TToken, T>> Fix<TToken, TParam, T>(Func<Func<TParam, Parser<TToken, T>>, TParam, Parser<TToken, T>> function)
            => parameter => Delay(() => function(Fix(function), parameter));
    }
}
