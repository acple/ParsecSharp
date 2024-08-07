using System;
using System.Linq;
using ChainingAssertion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;

namespace UnitTest.ParsecSharp
{
    [TestClass]
    public class ParserTest
    {
        private const string _abcdEFGH = "abcdEFGH";

        private const string _123456 = "123456";

        private const string _commanum = "123,456,789";

        [TestMethod]
        public void AnyTest()
        {
            // 任意のトークンにマッチするパーサを作成します。
            // このパーサは入力が終端の場合にのみ失敗します。

            var parser = Any();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void TokenTest()
        {
            // 指定したトークンにマッチするパーサを作成します。
            // EqualityComparer<T>.Default を利用して一致判定を行います。

            var source = _abcdEFGH;
            var source2 = _123456;

            // トークン 'a' にマッチするパーサ。
            var parser = Token('a');

            parser.Parse(source).WillSucceed(value => value.Is('a'));

            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void EndOfInputTest()
        {
            // 入力の終端にマッチするパーサを作成します。

            var parser = EndOfInput();

            var source = string.Empty;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void NullTest()
        {
            // 空文字にマッチし、任意の状態において常に成功するパーサを作成します。
            // このパーサは入力を消費しません。

            var parser = Null();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = string.Empty;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void OneOfTest()
        {
            // トークンが指定したシーケンスに含まれている場合に成功するパーサを作成します。

            // トークンが '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e' のいずれかに該当した場合に成功するパーサ。
            var parser = OneOf("6789abcde");

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();

            // params char[] を取るオーバーロード。IEnumerable<char> も受け取れる。
            var parser2 = OneOf('6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e');
            parser2.Parse(source).WillSucceed(value => value.Is('a'));

            parser2.Parse(source2).WillFail();
        }

        [TestMethod]
        public void NoneOfTest()
        {
            // トークンが指定したシーケンスに含まれていない場合に成功するパーサを作成します。

            var source = _abcdEFGH;
            var source2 = _123456;

            // トークンが 'd', 'c', 'b', 'a', '9', '8', '7' のいずれにも該当しない場合に成功するパーサ。
            var parser = NoneOf("dcba987");

            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            parser.Parse(source2).WillSucceed(value => value.Is('1'));

            // params char[] を取るオーバーロード。IEnumerable<char> も受け取れる。
            var parser2 = NoneOf('d', 'c', 'b', 'a', '9', '8', '7');

            parser2.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            parser2.Parse(source2).WillSucceed(value => value.Is('1'));
        }

        [TestMethod]
        public void TakeTest()
        {
            // 指定した数だけ入力を読み進め、その結果をシーケンスとして返すパーサを作成します。
            // 0を指定した場合は入力を消費せずに成功します。
            // 0未満の値を指定した場合は常に失敗します。

            // 3トークン読み進めるパーサ。
            var parser = Take(3);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1', '2', '3'));

            // 残りの入力よりも大きい数値を指定していた場合は失敗します。
            var parser2 = Take(9);
            parser2.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));
        }

        [TestMethod]
        public void SkipTest()
        {
            // 数を指定してスキップするパーサを作成します。
            // 0を指定した場合は入力を消費せずに成功します。
            // 0未満の値を指定した場合は常に失敗します。

            var source = _abcdEFGH;

            // 3トークンスキップした後次のトークンを返すパーサ。
            var parser = Skip(3).Right(Any());

            parser.Parse(source).WillSucceed(value => value.Is('d'));

            var parser2 = Skip(8).Right(EndOfInput());
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            // 指定数スキップできなかった場合は失敗します。
            var parser3 = Skip(9);
            parser3.Parse(source).WillFail(failure => failure.Message.Is("An input does not have enough length"));
        }

        [TestMethod]
        public void TakeWhileTest()
        {
            // 与えた条件を満たす限り入力を読み続けるパーサを作成します。

            // トークンが Lower である限り入力を読み続けるパーサ。
            var parser = TakeWhile(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Enumerable.Empty<char>()));
        }

        [TestMethod]
        public void TakeWhile1Test()
        {
            // 与えた条件を満たす限り入力を読み続けるパーサを作成します。
            // 1件もマッチしなかった場合、失敗を返します。

            // トークンが Lower である限り入力を読み続けるパーサ。
            var parser = TakeWhile1(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipWhileTest()
        {
            // 与えた条件を満たす限り入力を消費し続け、その結果を破棄するパーサを作成します。
            // TakeWhile と同様に動作しますが、結果を収集しないため高効率で動作します。

            // トークンが Lower である限り入力を消費し続けるパーサ。
            var parser = SkipWhile(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void SkipWhile1Test()
        {
            // 与えた条件を満たす限り入力を消費し続け、その結果を破棄するパーサを作成します。
            // 1件以上スキップできない場合、失敗を返します。

            // トークンが Lower である限り入力を消費し続けるパーサ。
            var parser = SkipWhile1(char.IsLower);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SatisfyTest()
        {
            // 入力を1つ取り、条件を満たす場合に成功するパーサを作成します。
            // 入力消費を伴う全てのパーサを構成可能です。

            var source = _abcdEFGH;

            // 'a' にマッチするパーサ。
            var parser = Satisfy(x => x == 'a'); // == Char('a');
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            // 'a', 'b', 'c' のいずれかにマッチするパーサ。
            var parser2 = Satisfy("abc".Contains); // == OneOf("abc");
            parser2.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void PureTest()
        {
            // 成功したという結果を返すパーサを作成します。
            // パーサに任意の値を投入する場合に使用します。
            // このパーサは入力を消費しません。

            var parser = Pure("success!");

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("success!"));

            // 値の生成をパーサ実行時まで遅延させる。
            var parser2 = Pure(_ => Unit.Instance);
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void FailTest()
        {
            // 失敗したという結果を返すパーサを作成します。
            // このパーサは入力を消費しません。

            var source = _abcdEFGH;

            var parser = Fail<Unit>();
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): Unexpected 'a<0x61>'"));

            // エラーメッセージを記述することができるオーバーロード。
            var parser2 = Fail<Unit>("errormessagetest");
            parser2.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): errormessagetest"));

            // パース失敗時の state をハンドル可能。
            var parser3 = Fail<Unit>(state => $"errormessagetest, current state: '{state}'");
            parser3.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): errormessagetest, current state: 'a<0x61>'"));
        }

        [TestMethod]
        public void AbortTest()
        {
            // 実行した時点でパース処理を中止するパーサを作成します。
            // 通常これを直接利用することはありません。AbortIfEntered コンビネータか AbortWhenFail コンビネータを利用します。

            // Abort または Any にマッチするが、Abort を評価した時点でパース処理が終了する。
            var parser = Abort<char>(_ => "aborted").Or(Any());

            var source = _123456;
            parser.Parse(source).WillFail(failure => failure.Message.Is("aborted"));
        }

        [TestMethod]
        public void GetPositionTest()
        {
            // パース位置の Position を取り出すパーサを作成します。
            // このパーサは入力を消費しません。

            // Anyに3回マッチした後、その時点の Position を返すパーサ。
            var parser = Any().Repeat(3).Right(GetPosition());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Column.Is(4));
        }

        [TestMethod]
        public void ChoiceTest()
        {
            // parsers を前から1つずつ適用し、最初に成功したものを結果として返すパーサを作成します。
            // 全て失敗した場合、最後の失敗を全体の失敗として返します。

            // 'c'、'b'、または 'a' のどれかにマッチするパーサ。
            var parser = Choice(Char('c'), Char('b'), Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SequenceTest()
        {
            // parsers に順にマッチングし、その結果を連結したシーケンスを返すパーサを作成します。

            // 'a' + 'b' + 'c' + 'd' にマッチし、['a', 'b', 'c', 'd'] を "abcd" に変換して返すパーサ。
            var parser = Sequence(Char('a'), Char('b'), Char('c'), Char('d')).AsString();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("abcd"));

            var source2 = "abCDEF";
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 3): Unexpected 'C<0x43>'"));
        }

        [TestMethod]
        public void TryTest()
        {
            // parser によるパースを実行し、それが失敗した場合は resume の値を結果として返す、常に成功するパーサを作成します。
            // parser のマッチング失敗時は入力を消費しません。

            // 'a' にマッチし、成功した場合は 'a'、失敗した場合は 'x' を返すパーサ。
            var parser = Try(Char('a'), 'x');

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('x'));

            // resume の評価を遅延させるオーバーロード。
            var parser2 = Try(Char('a'), _ => 'x');
            parser2.Parse(source).WillSucceed(value => value.Is('a'));
        }

        [TestMethod]
        public void OptionalTest()
        {
            // parser によるパースを実行し、マッチしたかどうかの bool 値を返す、常に成功するパーサを作成します。
            // parser のマッチング失敗時は入力を消費しません。

            var source = _abcdEFGH;
            var source2 = _123456;

            // Digit へのマッチングを行い、その値を破棄し、Any にマッチするパーサ。
            var parser = Optional(Digit()).Right(Any());

            parser.Parse(source).WillSucceed(value => value.Is('a'));

            parser.Parse(source2).WillSucceed(value => value.Is('2'));

            // 成功時には値を取り出し、失敗時には default value を結果とするオーバーロード。
            var parser2 = Optional(Lower(), '\n');

            parser2.Parse(source).WillSucceed(value => value.Is('a'));

            parser2.Parse(source2).WillSucceed(value => value.Is('\n'));
        }

        [TestMethod]
        public void NotTest()
        {
            // parser が失敗した場合に成功するパーサを作成します。
            // このパーサは入力を消費しません。

            // トークンが Lower でない場合に成功するパーサ。
            var parser = Not(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillFail();

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void LookAheadTest()
        {
            // 入力を消費せずに parser によるパースを行うパーサを作成します。

            // 入力を消費せずに Any *> Letter にマッチし、その後 Any とマッチしその結果を連結するパーサ。
            var parser = LookAhead(Any().Right(Letter())).Append(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b', 'a'));

            var source2 = _123456;
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): At LookAhead -> Parser Failure (Line: 1, Column: 2): Unexpected '2<0x32>'"));
        }

        [TestMethod]
        public void ManyTest()
        {
            // parser に0回以上繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // 1回もマッチしなかった場合、パーサは空のシーケンスを結果として返します。その場合は入力を消費しません。

            // Lower に0回以上繰り返しマッチするパーサ。
            var parser = Many(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void Many1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // 1回もマッチしなかった場合、パーサは失敗を返します。

            // Lower に1回以上繰り返しマッチするパーサ。
            var parser = Many1(Lower());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipManyTest()
        {
            // parser に0回以上繰り返しマッチし、その結果を破棄するパーサを作成します。
            // 1回もマッチしなかった場合は入力を消費しません。

            // Lower に0回以上繰り返しマッチし、その結果を破棄し、Any にマッチした結果を返すパーサ。
            var parser = SkipMany(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1'));
        }

        [TestMethod]
        public void SkipMany1Test()
        {
            // parser に1回以上繰り返しマッチし、その結果を破棄するパーサを作成します。

            // Lower に1回以上繰り返しマッチし、その結果を破棄し、Any にマッチした結果を返すパーサ。
            var parser = SkipMany1(Lower()).Right(Any());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void ManyTillTest()
        {
            // terminator にマッチするまでの間 parser に繰り返しマッチし、その結果をシーケンスとして返すパーサを作成します。
            // terminator にマッチした結果は破棄されます。

            var source = _abcdEFGH;
            var source2 = _123456;

            // 'F' にマッチするまでの間 Any に繰り返しマッチするパーサ。
            var parser = ManyTill(Any(), Char('F'));

            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E'));

            // terminator にマッチしなかった場合、失敗を返します。
            parser.Parse(source2).WillFail();

            // 最低1回以上 parser にマッチすることを保証する場合は Many1Till を利用する。
            var parser2 = Many1Till(Letter(), EndOfInput());

            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));

            // Letter にマッチしないため失敗。
            parser2.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SkipTillTest()
        {
            // terminator にマッチするまで parser に繰り返しマッチし、terminator にマッチした結果を返すパーサを作成します。

            // "cd" にマッチするまでの間 Lower をスキップするパーサ。
            var parser = SkipTill(Lower(), String("cd"));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("cd"));

            // "cd" の前に Upper が存在するため失敗する。
            var source2 = "xyzABcdef";
            parser.Parse(source2).WillFail();

            // 最低1回以上 parser にマッチすることを保証する場合は Skip1Till を利用する。
            var parser2 = Skip1Till(Letter(), EndOfInput());
            parser2.Parse(source).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void TakeTillTest()
        {
            // terminator にマッチするまで入力を読み、その結果をシーケンスとして返すパーサを作成します。
            // terminator にマッチした結果は破棄されます。消費したくない場合は LookAhead コンビネータを併用してください。

            // 'E' にマッチするまで入力を読むパーサ。
            var parser = TakeTill(Char('E'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd'));

            // 最後までマッチしない terminator を与えた場合、ストリームを最後まで読むことになるため、
            // パフォーマンスに影響を与える場合があります。
            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void MatchTest()
        {
            // parser にマッチするまでスキップし、parser にマッチした結果を返すパーサを作成します。

            // "FG" にマッチするまでスキップするパーサ。
            var parser = Match(String("FG"));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("FG"));

            var source2 = _123456;
            parser.Parse(source2).WillFail();

            // ( Lower Upper ) にマッチするまでスキップするパーサ。
            var parser2 = Match(Sequence(Lower(), Upper())).AsString();

            parser2.Parse(source).WillSucceed(value => value.Is("dE"));
        }

        [TestMethod]
        public void QuotedTest()
        {
            // 前後のパーサにマッチする間のトークン列を取得するパーサを作成します。
            // 文字列のようなトークンを取得する際に利用できます。
            // トークン列のマッチに条件を付ける場合は Quote 拡張メソッドが利用できます。

            // '<' と '>' に挟まれた文字列を取得するパーサ。
            var parser = Quoted(Char('<'), Char('>')).AsString();

            var source = "<abcd>";
            parser.Parse(source).WillSucceed(value => value.Is("abcd"));

            // '<' と '>' に挟まれた文字列、に挟まれた文字列を返すパーサ。
            var parser2 = Quoted(parser).AsString();

            var source2 = "<span>test</span>";
            parser2.Parse(source2).WillSucceed(value => value.Is("test"));
        }

        [TestMethod]
        public void AtomTest()
        {
            // 指定したパーサをパーサの最小単位として扱います。
            // パース処理が途中で失敗した場合であっても、起点までバックトラックを行います。
            // WithConsume / AbortIfEntered と組み合わせて使用します。

            var abCD = Sequence(Char('a'), Char('b'), Char('C'), Char('D'));
            var parser = Atom(abCD);

            var source = _abcdEFGH;
            abCD.Parse(source).WillFail(failure => failure.State.Position.Is(position => position.Line == 1 && position.Column == 3));
            parser.Parse(source).WillFail(failure => failure.State.Position.Is(position => position.Line == 1 && position.Column == 1));
        }

        [TestMethod]
        public void DelayTest()
        {
            // 指定したパーサの組み立てをパース実行時まで遅延します。
            // 参照保持としても使えるため、前方参照や再帰等を行う場合に必須となります。

            // 'a' にマッチするパーサ。ただしパース実行時に組み立てられる。
            var parser = Delay(() => Char('a'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            // Delay に渡す Func<T> はパース実行時に一度だけ実行されます。
            // そのため、処理が副作用を含む場合に意図しない動作となるため注意が必要です。
            // このパーサオブジェクトを再利用することでパフォーマンスを向上できます。
            // 利用例の詳細については Examples を参照。
        }

        [TestMethod]
        public void FixTest()
        {
            // ローカル変数上やパーサ定義式内に自己再帰パーサを構築する際のヘルパコンビネータです。
            // C# の仕様上、単体で使用する場合は型情報が不足するため、型引数を与える必要があります。

            // 任意の回数の "{}" に挟まれた一文字にマッチするパーサ。
            var parser = Fix<char>(self => self.Or(Any()).Between(Char('{'), Char('}')));

            var source = "{{{{{*}}}}}";
            parser.Parse(source).WillSucceed(value => value.Is('*'));

            // パラメータを取るオーバーロード。柔軟に再帰パーサを記述できます。
            // 有名な回文パーサ。 S ::= "a" S "a" | "b" S "b" | ""
            var parser2 = Fix<Parser<char, Unit>, Unit>((self, rest) =>
                Char('a').Right(self(Char('a').Right(rest))) | Char('b').Right(self(Char('b').Right(rest))) | rest);

            var source2 = "abbaabba";
            parser2(EndOfInput()).Parse(source2).WillSucceed(value => value.Is(Unit.Instance));
        }

        [TestMethod]
        public void NextTest()
        {
            // parser に対する継続処理を直接記述可能なコンビネータです。
            // 継続として処理されるため、自己再帰型のパーサ定義に利用することでパフォーマンスの低下を回避できます。

            // Letter にマッチした場合、次の Letter にマッチした結果を返し、失敗した場合は '\n' を返すパーサ。
            var parser = Letter().Next(_ => Letter(), '\n');

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b'));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('\n'));

            // parser が成功し、next が失敗したため全体の結果は失敗となる。
            var source3 = "a123";
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void GuardTest()
        {
            // parser がマッチした結果に対して条件式で成功と失敗を分岐します。

            // 数値にマッチし、1000 未満の場合のみ成功とするパーサ。
            var parser = Many1(DecDigit()).ToInt().Guard(x => x < 1000);

            var source = _123456;
            parser.Parse(source).WillFail(failure => failure.Message.Is("A value '123456' does not satisfy condition"));

            var source2 = "999";
            parser.Parse(source2).WillSucceed(value => value.Is(999));

            // parser がマッチしない場合は検証自体が行われない。
            var source3 = _abcdEFGH;
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void SeparatedByTest()
        {
            // separator によって区切られた形の parser が0回以上繰り返す入力にマッチするパーサを作成します。
            // separator にマッチした結果は破棄されます。

            // [ 1*Number *( "," 1*Number ) ]
            var parser = Many1(Number()).AsString().SeparatedBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void SeparatedBy1Test()
        {
            // separator によって区切られた形の parser が1回以上繰り返す入力にマッチするパーサを作成します。

            // 1*Number *( "," 1*Number )
            var parser = Many1(Number()).AsString().SeparatedBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _abcdEFGH;
            parser.Parse(source3).WillFail();
        }

        [TestMethod]
        public void EndByTest()
        {
            // 末尾に separator が付いた形の parser が0回以上繰り返すものにマッチするパーサを作成します。

            // *( 1*Number "," )
            var parser = Many1(Number()).AsString().EndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.IsEmpty());
        }

        [TestMethod]
        public void EndBy1Test()
        {
            // 末尾に separator が付いた形の parser が1回以上繰り返すものにマッチするパーサを作成します。

            // 1*( 1*Number "," )
            var parser = Many1(Number()).AsString().EndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456"));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void SeparatedOrEndByTest()
        {
            // SeparatedBy、または Endby のどちらかとして振る舞うパーサを作成します。

            // [ 1*Number *( "," 1*Number ) [ "," ] ]
            var parser = Many1(Number()).AsString().SeparatedOrEndBy(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).WillSucceed(value => value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void SeparatedOrEndBy1Test()
        {
            // SeparatedBy1、または Endby1 のどちらかとして振る舞うパーサを作成します。

            // 1*Number *( "," 1*Number ) [ "," ]
            var parser = Many1(Number()).AsString().SeparatedOrEndBy1(Char(','));

            var source = _commanum;
            parser.Parse(source).WillSucceed(value => value.Is("123", "456", "789"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is("123456"));

            var source3 = _commanum + ",";
            parser.Parse(source3).WillSucceed(value => value.Is("123", "456", "789"));
        }

        [TestMethod]
        public void ExceptTest()
        {
            // 指定したパーサに対して除外条件を設定したパーサを作成します。

            var source = _123456;

            // '5' 以外の数字にマッチするパーサ。
            var parser = Digit().Except(Char('5'));
            parser.Parse(source).WillSucceed(value => value.Is('1'));

            // '5' 以外の数字に連続でマッチし、文字列に変換したものを返すパーサ。
            var parser2 = Many(parser).AsString();
            parser2.Parse(source).WillSucceed(value => value.Is("1234"));
        }

        [TestMethod]
        public void ChainTest()
        {
            // 単一パーサから開始し、その結果を元に次のパーサを作成し、失敗するまで繰り返す再帰パーサを作成します。

            // 任意の一文字に連続でマッチし、マッチした文字とその回数を結果として返すパーサ。
            var parser = Any().Map(x => (x, count: 1))
                .Chain(match => Char(match.x).Map(_ => (match.x, match.count + 1)))
                .Map(match => match.x.ToString() + match.count.ToString());

            var source = "aaaaaaaaa";
            parser.Parse(source).WillSucceed(value => value.Is("a9"));

            var source2 = "aaabbbbcccccdddddd";
            Many(parser).Join().Parse(source2).WillSucceed(value => value.Is("a3b4c5d6"));

            // 本来、自己を最初に参照するパーサを直接記述することはできない(無限再帰となるため)。
            // 有名な二項演算の左再帰定義。
            // expr = expr op digit / digit
            static Parser<char, int> Expr()
                => (from x in Expr() // ここで無限再帰
                    from func in Char('+')
                    from y in Num()
                    select x + y)
                    .Or(Num());

            static Parser<char, int> Num()
                => Many1(Digit()).ToInt();

            // この定義を変形して左再帰を除去することが可能。
            // 二項演算の左再帰除去後の定義。
            // expr = digit *( op digit )
            static Parser<char, int> Expr2()
                => Num().Chain(x => Char('+').Right(Num()).Map(y => x + y));
            // Chain を使うことで左再帰除去後の定義をそのまま記述できる。

            Expr2().Parse("1+2+3+4").Value.Is(1 + 2 + 3 + 4);
        }

        [TestMethod]
        public void ChainLeftTest()
        {
            // 1個以上の値と演算子に交互にマッチし、指定した演算を左から順に適用するパーサを作成します。

            // '+'、または '-' にマッチし、それぞれ (x + y)、(x - y) の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

            // 1文字以上の数字にマッチし、int に変換するパーサ。
            var num = Many1(Digit()).ToInt();

            // num *( op num )
            var parser = num.ChainLeft(op);

            var source = "10+5-3+1";
            parser.Parse(source).WillSucceed(value => value.Is(((10 + 5) - 3) + 1));

            var source2 = "100-20-5+50";
            parser.Parse(source2).WillSucceed(value => value.Is(((100 - 20) - 5) + 50));

            var source3 = "123";
            parser.Parse(source3).WillSucceed(value => value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).WillFail();

            var source5 = "1-2+3+ABCD";
            parser.Parse(source5).WillSucceed(value => value.Is((1 - 2) + 3));
        }

        [TestMethod]
        public void ChainRightTest()
        {
            // 1個以上の値と演算子に交互にマッチし、指定した演算を右から順に適用するパーサを作成します

            // '+'、または '-' にマッチし、それぞれ (x + y)、(x - y) の二項演算関数を返すパーサ。
            // ( "+" / "-" )
            var op = Choice(
                Char('+').Map(_ => (Func<int, int, int>)((x, y) => x + y)),
                Char('-').Map(_ => (Func<int, int, int>)((x, y) => x - y)));

            // 1文字以上の数字にマッチし、int に変換するパーサ。
            var num = Many1(Digit()).ToInt();

            // num *( op num )
            var parser = num.ChainRight(op);

            var source = "10+5-3+1";
            parser.Parse(source).WillSucceed(value => value.Is(10 + (5 - (3 + 1))));

            var source2 = "100-20-5+50";
            parser.Parse(source2).WillSucceed(value => value.Is(100 - (20 - (5 + 50))));

            var source3 = "123";
            parser.Parse(source3).WillSucceed(value => value.Is(123));

            var source4 = _abcdEFGH;
            parser.Parse(source4).WillFail();

            var source5 = "1-2+3+ABCD";
            parser.Parse(source5).WillSucceed(value => value.Is(1 - (2 + 3)));
        }

        [TestMethod]
        public void FoldLeftTest()
        {
            // 初期値と集計関数を引数にとり、パースした結果を左から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して左から (x => accumulator - x) を繰り返し適用するパーサ。
            var parser = Digit().AsString().ToInt().FoldLeft(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).WillSucceed(value => value.Is(((((10 - 1) - 2) - 3) - 4) - 5));

            // 初期値を用いないオーバーロード。
            var parser2 = Digit().AsString().ToInt().FoldLeft((x, y) => x - y);
            parser2.Parse(source).WillSucceed(value => value.Is((((1 - 2) - 3) - 4) - 5));
        }

        [TestMethod]
        public void FoldRightTest()
        {
            // 初期値と集計関数を引数にとり、パース結果を右から集計するパーサを作成します。

            // 0個以上の Digit にマッチし、初期値10に対して右から (x => x - accumulator) を繰り返し適用するパーサ。
            var parser = Digit().AsString().ToInt().FoldRight(10, (x, y) => x - y);

            var source = "12345";
            parser.Parse(source).WillSucceed(value => value.Is(1 - (2 - (3 - (4 - (5 - 10))))));

            // 初期値を用いないオーバーロード。
            var parser2 = Digit().AsString().ToInt().FoldRight((x, y) => x - y);
            parser2.Parse(source).WillSucceed(value => value.Is(1 - (2 - (3 - (4 - 5)))));
        }

        [TestMethod]
        public void RepeatTest()
        {
            // parser を count 回繰り返しマッチした結果をシーケンスとして返すパーサを作成します。

            // 2*( 3*Any )
            var parser = Any().Repeat(3).AsString().Repeat(2);

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is("abc", "dEF"));
        }

        [TestMethod]
        public void LeftTest()
        {
            // 二つのパーサに順にマッチングし、right の結果を破棄して left の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'a'を返すパーサ。
            var parser = Char('a').Left(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('a'));

            var source2 = "a";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void RightTest()
        {
            // 二つのパーサに順にマッチングし、left の結果を破棄して right の結果を返すパーサを作成します。

            // ( "a" "b" )にマッチし、'b'を返すパーサ。
            var parser = Char('a').Right(Char('b'));

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is('b'));

            var source2 = "b";
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void BetweenTest()
        {
            // parser を open と close で挟み込んだ形の規則にマッチするパーサを作成します。
            // open と close の結果は破棄され、中央の結果のみを返します。

            // 1文字以上の Letter を "[]" で包んだものにマッチするパーサ。
            // ( "[" 1*Letter "]" )
            var parser = Many1(Letter()).Between(Char('['), Char(']'));

            var source = $"[{_abcdEFGH}]";
            parser.Parse(source).WillSucceed(value => value.Is(_abcdEFGH));

            // Many(Any()) などを parser に渡した場合、終端まで Any にマッチするため、 close は EndOfInput にマッチします。
            var parser2 = Many(Any()).Between(Char('"'), Char('"')); // ( dquote *Any dquote ) とはならない
            parser2.Parse(@"""abCD1234""").WillFail(); // Many(Any()) が abCD1234" までマッチしてしまうため、close の " がマッチせず失敗する
            // この形にマッチするパーサを作成したいときは、Quoted や ManyTill の使用を検討してください。
        }

        [TestMethod]
        public void QuoteTest()
        {
            // 前後のパーサにマッチするまで parser に連続でマッチするパーサを作成します。

            // '"' をエスケープ可能な文字列表現にマッチするパーサ。
            var dquoteOrAny = String("\\\"").Map(_ => '\"') | Any();
            var parser = dquoteOrAny.Quote(Char('"')).AsString();

            var source = "\"abcd\\\"EFGH\"";
            parser.Parse(source).WillSucceed(value => value.Is("abcd\"EFGH"));
        }

        [TestMethod]
        public void AppendTest()
        {
            // 2つのパーサに順にマッチングし、その結果を結合したシーケンスを返します。

            var source = _abcdEFGH;

            {
                // 1文字 + 1文字
                var parser = Any().Append(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("ab"));
                var source2 = "a";
                parser.Parse(source2).WillFail();
            }

            {
                // 小文字*n + 1文字
                var parser = Many1(Lower()).Append(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdE"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }

            {
                // 1文字 + 小文字*n
                var parser = Any().Append(Many1(Lower())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcd"));
                var source2 = "ABCD";
                parser.Parse(source2).WillFail();
            }

            {
                // 小文字*n + 大文字*n
                var parser = Many1(Lower()).Append(Many1(Upper())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillFail();
            }
        }

        [TestMethod]
        public void AppendOptionalTest()
        {
            // Append と同様に振る舞いますが、右のパーサが失敗した場合でもその結果を空のシーケンスとして扱い合成します。

            var source = _abcdEFGH;

            {
                // 1文字 + 1文字
                var parser = Any().AppendOptional(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("ab"));
                var source2 = "a";
                parser.Parse(source2).WillSucceed(value => value.Is("a"));
            }

            {
                // 小文字*n + 1文字
                var parser = Many1(Lower()).AppendOptional(Any()).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdE"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }

            {
                // 1文字 + 小文字*n
                var parser = Any().AppendOptional(Many1(Lower())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcd"));
                var source2 = "ABCD";
                parser.Parse(source2).WillSucceed(value => value.Is("A"));
            }

            {
                // 小文字*n + 大文字*n
                var parser = Many1(Lower()).AppendOptional(Many1(Upper())).AsString();
                parser.Parse(source).WillSucceed(value => value.Is("abcdEFGH"));
                var source2 = "abcd";
                parser.Parse(source2).WillSucceed(value => value.Is("abcd"));
            }
        }

        [TestMethod]
        public void IgnoreTest()
        {
            // パースした結果を破棄します。
            // 型合わせや、値を捨てることを明示したい場合に使用します。

            // 1文字以上の小文字にマッチし、その結果を破棄するパーサ。
            var parser = Many1(Lower()).Ignore();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(Unit.Instance));

            parser.Right(Any()).Parse(source).WillSucceed(value => value.Is('E'));

            var source2 = _123456;
            parser.Parse(source2).WillFail();
        }

        [TestMethod]
        public void EndTest()
        {
            // parser が入力を終端まで消費しきったことを保証するコンビネータです。
            // 終端に到達していなかった場合に失敗となります。

            var source = _abcdEFGH;

            // 1文字以上の小文字にマッチし、その時点で全ての入力を消費していなければならないパーサ。
            var parser = Many1(Lower()).End();
            parser.Parse(source).WillFail(failure => failure.Message.Is("Expected '<EndOfStream>' but was 'E<0x45>'"));

            // 1文字以上の小文字または大文字にマッチし、その時点ですべての入力を消費していなければならないパーサ。
            var parser2 = Many1(Lower() | Upper()).End();
            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));
        }

        [TestMethod]
        public void FlattenTest()
        {
            // 二重の IEnumerable を結果に持つパーサに対して、一つに潰すコンビネータです。

            var source = _abcdEFGH;

            // 2つのトークンを取るパーサ。
            var token = Any().Repeat(2);
            // 2つのトークンを取るパーサに1回以上繰り返しマッチするパーサ。
            var parser = Many1(token);
            // parser の結果を潰したパーサ。
            var parser2 = parser.Flatten();

            parser.Parse(source).WillSucceed(value => value.Is(value => value.Count() == 4 && value.All(x => x.Count() == 2)));

            parser2.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));

            // Many1 を用いたために二重になってしまうような状況では、代わりに FoldLeft の利用が検討できます。
            var parser3 = token.FoldLeft((x, y) => x.Concat(y));
            parser3.Parse(source).WillSucceed(value => value.Is('a', 'b', 'c', 'd', 'E', 'F', 'G', 'H'));
        }

        [TestMethod]
        public void SingletonTest()
        {
            // parser にマッチした結果を1要素のシーケンスとして返すコンビネータです。

            // 'a' にマッチし、[ 'a' ] を返すパーサ。
            var parser = Char('a').Singleton();

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Count().Is(1));
        }

        [TestMethod]
        public void WithConsumeTest()
        {
            // parser が入力を消費せずに成功した場合、それを失敗として扱うパーサを作成します。
            // Many 等に入力を消費しない可能性のあるパーサを渡す場合に利用できます。

            // Letter にマッチしなかった場合に発生する無限ループを回避したパーサ。
            var parser = Many1(Many(Letter()).WithConsume().AsString());

            var source = _abcdEFGH;
            parser.Parse(source).WillSucceed(value => value.Is(_abcdEFGH));

            var source2 = _123456;
            parser.Parse(source2).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): A parser did not consume any input"));
        }

        [TestMethod]
        public void WithMessageTest()
        {
            // パース失敗時のエラーメッセージを書き換えます。

            var parser = Many1(Digit())
                .WithMessage(failure => $"MessageTest Current: '{failure.State.Current}', original message: {failure.Message}");

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 1): MessageTest Current: 'a', original message: Unexpected 'a<0x61>'"));

            var source2 = _123456;
            parser.Parse(source2).WillSucceed(value => value.Is('1', '2', '3', '4', '5', '6'));
        }

        [TestMethod]
        public void AbortWhenFailTest()
        {
            // パース失敗時にパース処理を中止します。

            var parser = Many(Lower().AbortWhenFail(failure => $"Fatal Error! '{failure.State.Current}' is not a lower char!")).AsString()
                .Or(Pure("recovery"));

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.ToString().Is("Parser Failure (Line: 1, Column: 5): Fatal Error! 'E' is not a lower char!"));
        }

        [TestMethod]
        public void AbortIfEnteredTest()
        {
            // パーサが入力を消費した状態で失敗した場合にパース処理を中止します。
            // LL(k) パーサのような失敗時の早期脱出を実現します。

            var parser = Sequence(Char('1'), Char('2'), Char('3'), Char('4')).AsString().AbortIfEntered(_ => "abort1234")
                .Or(Pure("recovery"));

            var source = _123456;
            parser.Parse(source).WillSucceed(value => value.Is("1234"));

            var source2 = _abcdEFGH;
            parser.Parse(source2).WillSucceed(value => value.Is("recovery"));

            var source3 = _commanum;
            parser.Parse(source3).WillFail(failure => failure.Message.Is("abort1234")); // 123まで入力を消費して失敗したため復旧が行われない
        }

        [TestMethod]
        public void DoTest()
        {
            // パース実行時に定義したアクションを実行します。
            // 成功時、または 成功時と失敗時にそれぞれ指定したアクションを動作させることができます。

            var source = _abcdEFGH;

            // Lower のパース成功時に count の値を1増やします。
            var count = 0;
            var parser = Many(Lower().Do(_ => count++));

            _ = parser.Parse(source);
            count.Is(4);
            _ = parser.Parse(source);
            count.Is(8);

            // Lower のパース成功時に success の値を1増やし、パース失敗時に failure の値を1増やします。
            // Any を接続しているので、source を最後までパースします。
            var success = 0;
            var failure = 0;
            var parser2 = Many(Lower().Do(_ => success++, _ => failure++).Or(Any()));

            _ = parser2.Parse(source);
            success.Is(4);
            failure.Is(5);
        }

        [TestMethod]
        public void ExceptionTest()
        {
            // 処理中に例外が発生した場合は即座にパース処理を中止し、例外の Name をメッセージに含む Failure を返します。
            // 例外による失敗に対して復旧は行われません。復旧を行う手段はありません。

            // null に対して ToString した結果を返し、失敗した場合に "success" を返すことを試みるパーサ。
            var parser = Pure(null as object).Map(x => x!.ToString()!).Or(Pure("success"));

            var source = _abcdEFGH;
            parser.Parse(source).WillFail(failure => failure.ToString().Is(message => message.Contains("Exception 'NullReferenceException' occurred:")));
        }

        [TestMethod]
        public void ParsePartiallyTest()
        {
            // パース完了後にストリームを破棄せず、続きの処理を行うことができる実行プランを提供します。

            // 3文字消費するパーサ。
            var parser = Any().Repeat(3).AsString();

            using var source = StringStream.Create(_abcdEFGH);

            var (result, rest) = parser.ParsePartially(source);
            result.WillSucceed(value => value.Is("abc"));

            var (result2, rest2) = parser.ParsePartially(rest);
            result2.WillSucceed(value => value.Is("dEF"));

            var (result3, rest3) = parser.ParsePartially(rest2);
            result3.WillFail(failure => failure.Message.Is("Unexpected '<EndOfStream>'")); // 終端に到達したため失敗

            // 失敗時点の state が返されることに注意する。
            EndOfInput().Parse(rest3).WillSucceed(value => value.Is(Unit.Instance));
        }
    }
}
