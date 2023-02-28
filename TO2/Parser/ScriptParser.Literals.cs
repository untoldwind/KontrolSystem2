using System;
using System.Globalization;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsers;
    using static TO2ParserCommon;

    public static class TO2ParserLiterals {
        private static readonly Parser<char> DoubleQuote = Char('"');

        private static readonly Parser<char> EscapedStringChar = Alt(
            CharExcept("\\\"\r\n"),
            Tag("\\\"").To('"'),
            Tag("\\t").To('\t'),
            Tag("\\n").To('\n'),
            Tag("\\r").To('\r')
        );

        public static readonly Parser<LiteralString> LiteralString = Many0(EscapedStringChar)
            .Between(DoubleQuote, DoubleQuote)
            .Map((chars, start, end) => new LiteralString(chars.ToArray(), start, end)).Named("<string>");

        private static readonly Parser<int> BasePrefix = Alt(
            Tag("0x").To(16),
            Tag("0o").To(8),
            Tag("0b").To(2),
            Value(10)
        );

        public static readonly Parser<LiteralInt> LiteralInt = Seq(
            Opt(Char('-')), BasePrefix, Recognize(Digits1.Then(Chars0(ch => char.IsDigit(ch) || ch == '_')))
        ).Map((items, start, end) =>
            new LiteralInt(
                items.Item1.IsDefined
                    ? -Convert.ToInt64(items.Item3.Replace("_", ""), items.Item2)
                    : Convert.ToInt64(items.Item3.Replace("_", ""), items.Item2), start, end)).Named("<integer>");

        private static readonly Parser<string> ExponentSuffix = OneOf("eE").Then(Opt(OneOf("+-"))).Then(Digits1);

        public static readonly Parser<LiteralFloat> LiteralFloat = Recognize(
            Opt(OneOf("+-")).Then(Alt(
                Terminated(Digits0.Then(Char('.')).Then(Digits1), Opt(ExponentSuffix)),
                Digits1.Then(ExponentSuffix)
            ))
        ).Map((digits, start, end) =>
            new LiteralFloat(Convert.ToDouble(digits, CultureInfo.InvariantCulture), start, end)).Named("<float>");

        public static readonly Parser<LiteralBool> LiteralBool = Alt(
            Identifier.Where(str => str == "true", "true").Map((_, start, end) => new LiteralBool(true, start, end)),
            Identifier.Where(str => str == "false", "false").Map((_, start, end) => new LiteralBool(false, start, end))
        );
    }
}
