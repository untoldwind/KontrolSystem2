using System.Collections.Generic;
using System.IO;
using System.Text;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser;

using static Parsers;

public static class TO2ParserCommon {
    private static readonly HashSet<string> ReservedKeywords = new() {
        "pub", "fn", "let", "const", "if", "else", "return", "break", "continue", "while", "_", "for", "in",
        "as", "sync", "type", "struct", "impl"
    };

    public static readonly Parser<string> PubKeyword = Tag("pub").Then(Spacing1);

    public static readonly Parser<string> LetKeyword = Tag("let").Then(Spacing1);

    public static readonly Parser<string> ConstKeyword = Tag("const").Then(Spacing1);

    public static readonly Parser<LineComment> LineComment =
        CharsExcept0("\r\n").Map((comment, start, end) => new LineComment(comment, start, end))
            .Between(WhiteSpaces0.Then(Tag("//")), PeekLineEnd);

    public static readonly Parser<List<LineComment>> LineComments = Delimited0(LineComment, WhiteSpaces0);

    public static readonly Parser<char> CommaDelimiter =
        Char(',').Between(LineComments.Then(WhiteSpaces0), LineComments.Then(WhiteSpaces0));

    public static readonly Parser<char> EqDelimiter = Char('=').Between(WhiteSpaces0, WhiteSpaces0);

    public static readonly Parser<string> Identifier = Recognize(
        Char(ch => char.IsLetter(ch) || ch == '_', "letter or _")
            .Then(Chars0(ch => char.IsLetterOrDigit(ch) || ch == '_'))
    ).Where(s => !ReservedKeywords.Contains(s), "Not a keyword").Named("<identifier>");

    public static readonly Parser<List<string>> IdentifierPath = Delimited1(Identifier, Tag("::"));

    public static readonly Parser<TO2Type> TypeRef = TypeRefImpl;

    public static readonly Parser<TO2Type> TypeSpec = WhiteSpaces0.Then(Char(':')).Then(WhiteSpaces0).Then(TypeRef);

    private static readonly Parser<List<TO2Type>> FunctionTypeParameters = Char('(').Then(WhiteSpaces0)
        .Then(DelimitedUntil(TypeRef, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

    private static readonly Parser<TO2Type> FunctionType = Seq(
        Tag("fn").Then(WhiteSpaces0).Then(FunctionTypeParameters),
        WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(TypeRef)
    ).Map(items => new FunctionType(false, items.Item1, items.Item2));

    private static readonly Parser<TO2Type> TupleType =
        DelimitedN_M(2, null, Opt(LineComment.Then(WhiteSpaces0)).Then(TypeRef), CommaDelimiter, "<type>")
            .Between(Char('(').Then(WhiteSpaces0), LineComments.Then(WhiteSpaces0).Then(Char(')')))
            .Map(items => new TupleType(items));

    public static readonly Parser<TO2Type> RecordType =
        Delimited1(Opt(LineComment.Then(WhiteSpaces0)).Then(Seq(Identifier, TypeSpec)), CommaDelimiter,
                "<identifier : type>")
            .Between(Char('(').Then(WhiteSpaces0), LineComments.Then(WhiteSpaces0).Then(Char(')')))
            .Map(items => new RecordTupleType(items));

    private static readonly Parser<TO2Type> TypeReference = Seq(
        IdentifierPath,
        Opt(Delimited0(TypeRef, CommaDelimiter)
                .Between(WhiteSpaces0.Then(Char('<')).Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>'))))
            .Map(o => o.IsDefined ? o.Value : new List<TO2Type>())
    ).Map((items, start, end) => BuiltinType.GetBuiltinType(items.Item1, items.Item2) ??
                                 new LookupTypeReference(items.Item1, items.Item2, start, end));

    private static readonly Parser<TO2Type> ToplevelTypeRef = Seq(Alt(
        FunctionType,
        TupleType,
        RecordType,
        TypeReference
    ), Many0(Spacing0.Then(Char('[')).Then(Spacing0).Then(Char(']')))).Map(items => {
        if (items.Item2.Count > 0) return new ArrayType(items.Item1, items.Item2.Count);
        return items.Item1;
    });

    public static readonly Parser<DeclarationParameter> DeclarationParameter = Seq(
        Identifier, Opt(WhiteSpaces0.Then(Char('@')).Then(WhiteSpaces0).Then(Identifier)), Opt(TypeSpec)
    ).Map(items => {
        if (items.Item3.IsDefined)
            return new DeclarationParameter(items.Item1, items.Item2.GetOrElse(items.Item1), items.Item3.Value);
        return new DeclarationParameter(items.Item1, items.Item2.GetOrElse(items.Item1));
    });

    public static readonly Parser<DeclarationParameter> DeclarationParameterOrPlaceholder = Alt(
        DeclarationParameter,
        Char('_').Map(_ => new DeclarationParameter())
    );

    public static readonly Parser<string> DescriptionComment =
        Many0(CharsExcept0("\r\n").Map(s => s.Trim()).Between(WhiteSpaces0.Then(Tag("///")), PeekLineEnd))
            .Map(lines => string.Join("\n", lines));

    private static IResult<TO2Type> TypeRefImpl(IInput input) {
        return ToplevelTypeRef(input);
    }
}

public static class TO2Parser {
    private static IResult<TO2Module> TryParseModuleFile(string baseDir, string moduleFile) {
        var content = File.ReadAllText(Path.Combine(baseDir, moduleFile), Encoding.UTF8);
        var moduleResult =
            TO2ParserModule.Module(TO2Module.BuildName(moduleFile)).TryParse(content, moduleFile);
        if (!moduleResult.WasSuccessful)
            return Result.Failure<TO2Module>(moduleResult.Remaining, moduleResult.Expected);

        return Result.Success(moduleResult.Remaining, moduleResult.Value);
    }

    public static TO2Module ParseModuleFile(string baseDir, string moduleFile) {
        var result = TryParseModuleFile(baseDir, moduleFile);
        if (!result.WasSuccessful) throw new ParseException(result.Position, result.Expected);
        return result.Value;
    }
}
