using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Parsec;
using static Parsec.Parser;
using static Parsec.Text;

namespace ParsecSharpExamples
{
    public static class JsonParser
    {
        public static dynamic Parse(string json)
            => ParseJson().Parse(json).CaseOf(
                fail => null,
                success => success.Value);

        public static dynamic Parse(Stream json)
            => ParseJson().Parse(json).CaseOf(
                fail => null,
                success => success.Value);

        private static Parser<char, T> WithSpaces<T>(this Parser<char, T> parser)
            => parser.Between(Spaces(), Spaces());

        private static Parser<TToken, dynamic> AsDynamic<TToken, T>(this Parser<TToken, T> parser)
            => parser.FMap(x => x as dynamic);

        private static Parser<char, char> LeftBrace()
            => Char('{').WithSpaces();

        private static Parser<char, char> RightBrace()
            => Char('}').WithSpaces();

        private static Parser<char, char> LeftBracket()
            => Char('[').WithSpaces();

        private static Parser<char, char> RightBracket()
            => Char(']').WithSpaces();

        private static Parser<char, char> Comma()
            => Char(',').WithSpaces();

        private static Parser<char, char> Colon()
            => Char(':').WithSpaces();

        private static Parser<char, dynamic> ParseJson()
            => Choice(ParseObject(), ParseArray(), ParseString(), ParseNumber(), ParseBool(), ParseNull());

        private static Parser<char, dynamic> ParseObject()
            => ParseKeyValuePair().SepBy(Comma()).Between(LeftBrace(), RightBrace())
                .FMap(x => x.ToDictionary(item => item.Key, item => item.Value))
                .AsDynamic();

        private static Parser<char, KeyValuePair<string, dynamic>> ParseKeyValuePair()
            => from key in ParseString()
               from _ in Colon()
               from value in ParseJson()
               select new KeyValuePair<string, dynamic>(key, value);

        private static Parser<char, dynamic> ParseArray()
            => Delay(ParseJson).SepBy(Comma()).Between(LeftBracket(), RightBracket())
                .FMap(x => x.ToList())
                .AsDynamic();

        private static Parser<char, dynamic> ParseString()
            => Char('\"').Right(ManyTill(ParseJsonChar(), Char('\"'))).ToStr()
                .AsDynamic();

        private static Parser<char, char> ParseJsonChar()
            => Char('\\').Right(Choice(
                Char('\"').FMap(_ => '\"'),
                Char('\\').FMap(_ => '\\'),
                Char('/').FMap(_ => '/'),
                Char('b').FMap(_ => '\b'),
                Char('f').FMap(_ => '\f'),
                Char('n').FMap(_ => '\n'),
                Char('r').FMap(_ => '\r'),
                Char('t').FMap(_ => '\t'),
                Char('u').Right(HexDigit().Repeat(4).ToStr()
                    .FMap(x => (char)int.Parse(x, NumberStyles.HexNumber)))))
                .Or(Any());

        private static Parser<char, dynamic> ParseNumber()
            => (from neg in ParseSign()
                from integer in ParseInt()
                from frac in ParseFrac()
                from exp in ParseExp()
                select neg((integer + frac) * Math.Pow(10, exp)))
                .AsDynamic();

        private static Parser<char, Func<double, double>> ParseSign()
            => Try(Char('-').FMap(_ => new Func<double, double>(x => -x)),
                () => new Func<double, double>(x => x));

        private static Parser<char, int> ParseInt()
            => Char('0').ToStr().Or(OneOf("123456789").Append(Many(Digit())).ToStr())
                .FMap(x => int.Parse(x));

        private static Parser<char, double> ParseFrac()
            => Try(
                Char('.').Right(Many1(Digit())).ToStr().FMap(x => double.Parse("0." + x)),
                () => 0);

        private static Parser<char, int> ParseExp()
            => Try(from _ in OneOf("eE")
                   from sign in Char('-').FMap(__ => new Func<int, int>(x => -x))
                                    .Or(Optional(Char('+')).FMap(__ => new Func<int, int>(x => x)))
                   from num in Many1(Digit()).ToStr().FMap(x => int.Parse(x))
                   select sign(num),
                () => 0);

        private static Parser<char, dynamic> ParseBool()
            => Choice(
                String("false").FMap(_ => false),
                String("true").FMap(_ => true))
                .AsDynamic();

        private static Parser<char, dynamic> ParseNull()
            => String("null").FMap(_ => null as dynamic);
    }
}
