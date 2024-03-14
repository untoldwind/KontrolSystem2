using System;
using System.Globalization;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser;

using static Parsers;
using static TO2ParserCommon;

public static class TO2ParserLiterals {
    public static readonly Parser<char> DoubleQuote = Char('"');

    private static readonly Parser<char> EscapedStringChar = Alt(
        CharExcept("\\\"\r\n"),
        Tag("\\\\").To('\\'),
        Tag("\\\"").To('"'),
        Tag("\\t").To('\t'),
        Tag("\\n").To('\n'),
        Tag("\\r").To('\r')
    );

    public static readonly Parser<Expression> LiteralString = Many0(EscapedStringChar)
        .Between(DoubleQuote, DoubleQuote)
        .Map((chars, start, end) => new LiteralString(chars.ToArray(), start, end) as Expression).Named("<string>");

    private static bool IsHexDigit(char ch) => (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');

    private static bool IsOctalDigit(char ch) => ch >= '0' && ch <= '7';

    private static bool IsBinaryDigit(char ch) => ch == '0' || ch == '1';

    private static readonly Parser<(int fromBase, string str)> BasePrefixed = Alt(
        Preceded(Tag("0x"), Recognize(Chars1(IsHexDigit, "<hexdigit>").Then(Chars0(ch => IsHexDigit(ch) || ch == '_'))).Map(str => (16, str))),
        Preceded(Tag("0o"), Recognize(Chars1(IsOctalDigit, "<octal>").Then(Chars0(ch => IsOctalDigit(ch) || ch == '_'))).Map(str => (8, str))),
        Preceded(Tag("0b"), Recognize(Chars1(IsBinaryDigit, "<binary>").Then(Chars0(ch => IsBinaryDigit(ch) || ch == '_'))).Map(str => (2, str))),
        Recognize(Digits1.Then(Chars0(ch => char.IsDigit(ch) || ch == '_'))).Map(str => (10, str))
    );

    public static readonly Parser<Expression> LiteralInt = Seq(
        Opt(Char('-')), BasePrefixed
    ).Map((items, start, end) =>
        new LiteralInt(
            items.Item1.IsDefined
                ? -Convert.ToInt64(items.Item2.str.Replace("_", ""), items.Item2.fromBase)
                : Convert.ToInt64(items.Item2.str.Replace("_", ""), items.Item2.fromBase), start, end) as Expression).Named("<integer>");

    private static readonly Parser<bool> ExponentSuffix = OneOf("eE").Then(Opt(OneOf("+-"))).Then(Digits1);

    public static readonly Parser<Expression> LiteralFloat = Recognize(
        Opt(OneOf("+-")).Then(Alt(
            Terminated(Digits0.Then(Char('.')).Then(Digits1), Opt(ExponentSuffix)),
            Digits1.Then(ExponentSuffix)
        ))
    ).Map((digits, start, end) =>
        new LiteralFloat(Convert.ToDouble(digits, CultureInfo.InvariantCulture), start, end) as Expression).Named("<float>");

    public static readonly Parser<Expression> LiteralBool = Alt(
        Identifier.Where(str => str == "true", "true").Map((_, start, end) => new LiteralBool(true, start, end) as Expression),
        Identifier.Where(str => str == "false", "false").Map((_, start, end) => new LiteralBool(false, start, end) as Expression)
    );
}
