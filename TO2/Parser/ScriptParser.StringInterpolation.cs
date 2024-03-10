using System.Linq.Expressions;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using Expression = KontrolSystem.TO2.AST.Expression;

namespace KontrolSystem.TO2.Parser;

using static Parsers;
using static TO2ParserExpressions;

public static class TO2ParserStringInterpolation {
    public static readonly Parser<string> StringInterpolationStart = Tag("$\"");

    private static readonly Parser<char> ExtendedEscapedStringChar = Alt(
        CharExcept("\\\"\r\n{}"),
        Tag("\\\"").To('"'),
        Tag("\\t").To('\t'),
        Tag("\\n").To('\n'),
        Tag("\\r").To('\r'),
        Tag("\\{").To('{'),
        Tag("\\}").To('}')
    );

    public static IResult<Expression> StringInterpolationContentImpl(IInput input) => Many0(Alt<StringInterpolationPart>(
            Many1(ExtendedEscapedStringChar).Map(chars => new StringInterpolationPart.StringPart(new string(chars.ToArray()))),
            Expression.Between(Char('{'), Char('}')).Map(expr => new StringInterpolationPart.ExpressionPart(expr))
            )).Map((parts, start, end) => new StringInterpolation(parts, end, start))(input);
}
