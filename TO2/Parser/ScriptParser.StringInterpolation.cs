using System.Linq.Expressions;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using Expression = KontrolSystem.TO2.AST.Expression;

namespace KontrolSystem.TO2.Parser;

using static Parsers;
using static TO2ParserExpressions;
using static TO2ParserLiterals;

public static class TO2ParserStringInterpolation {
    private static readonly Parser<string> StringInterpolationStart = Tag("$\"");

    private static readonly Parser<char> ExtendedEscapedStringChar = Alt(
        CharExcept("\\\"\r\n{}"),
        Tag("\\\"").To('"'),
        Tag("\\t").To('\t'),
        Tag("\\n").To('\n'),
        Tag("\\r").To('\r'),
        Tag("\\{").To('{'),
        Tag("\\}").To('}')
    );

    private static readonly Parser<string> AlignOrFormat =
        Recognize(Seq(
            Opt(Char(',').Then(Opt(Char('-')).Then(Digits1))),
            Opt(Char(':').Then(CharsExcept1("\\\"\r\n{}", "align or format")))));

    private static readonly Parser<Expression> StringInterpolationContent = Many0(Alt<StringInterpolationPart>(
            Many1(ExtendedEscapedStringChar).Map(chars => new StringInterpolationPart.StringPart(new string(chars.ToArray()))),
            Seq(Expression, Opt(AlignOrFormat)).Between(Char('{').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('}'))).
                Map(expr => new StringInterpolationPart.ExpressionPart(expr.Item1, expr.Item2.GetOrElse("")))
        )).Map((parts, start, end) => new StringInterpolation(parts, end, start));

    public static readonly Parser<Expression> StringInterpolation =
        StringInterpolationContent.Between(StringInterpolationStart, DoubleQuote);

}
