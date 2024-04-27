﻿using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser;

using static Parsers;
using static TO2ParserCommon;
using static TO2ParserExpressions;
using static TO2ParserFunctions;

public static class TO2ParserModule {
    private static readonly Parser<bool> UseKeyword = Tag("use").Then(Spacing1);

    private static readonly Parser<bool> TypeKeyword = Tag("type").Then(Spacing1);

    private static readonly Parser<bool> StructKeyword = Tag("struct").Then(Spacing1);

    private static readonly Parser<bool> ImplKeyword = Tag("impl").Then(Spacing1);

    private static readonly Parser<bool> OperatorsKeyword = Tag("operators").Then(Spacing1.Then(Tag("for").Then(Spacing1)));

    private static readonly Parser<bool> FormKeyword = Spacing1.Then(Tag("from")).Then(Spacing1);

    private static readonly Parser<bool> AsKeyword = Spacing1.Then(Tag("as")).Then(Spacing1);

    private static readonly Parser<List<string>?> UseNames = Alt(
        Char('*').Map(_ => (List<string>?)null),
        Delimited1(Identifier, CommaDelimiter).Map(item => (List<string>?)item)
            .Between(Char('{').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('}')))
    );

    public static readonly Parser<UseDeclaration> UseNamesDeclaration = Seq(
        UseKeyword.Then(UseNames), FormKeyword.Then(IdentifierPath)
    ).Map((decl, start, end) =>
        new UseDeclaration(decl.Item1, decl.Item2, start, end));

    public static readonly Parser<UseDeclaration> UseAliasDeclaration = Seq(
        UseKeyword.Then(IdentifierPath), AsKeyword.Then(Identifier)
    ).Map((decl, start, end) => new UseDeclaration(decl.Item1, decl.Item2, start, end));

    public static readonly Parser<TypeAlias> TypeAlias = Seq(
        DescriptionComment, WhiteSpaces0.Then(Opt(PubKeyword)), TypeKeyword.Then(Identifier),
        EqDelimiter.Then(TypeRef)
    ).Map((items, start, end) =>
        new TypeAlias(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, start, end));

    private static readonly Parser<StructField> StructField = Seq(
        DescriptionComment, WhiteSpaces0.Then(Identifier), TypeSpec,
        EqDelimiter.Then(Expression)).Map((items, start, end) =>
        new StructField(items.Item2, items.Item3, items.Item1, items.Item4, start, end));

    private static readonly Parser<StructDeclaration> StructDeclaration = Seq(
        DescriptionComment, WhiteSpaces0.Then(Opt(PubKeyword)), StructKeyword.Then(Identifier),
        Opt(FunctionParameters),
        Delimited0(Either(LineComment, StructField), WhiteSpaces1, "fields")
            .Between(WhiteSpaces0.Then(Char('{')), WhiteSpaces0.Then(Char('}')))
    ).Map((items, start, end) =>
        new StructDeclaration(items.Item2.IsDefined, items.Item3, items.Item1,
            items.Item4.IsDefined ? items.Item4.Value : [], items.Item5, start, end));

    private static readonly Parser<IModuleItem> ImplDeclaration = ImplKeyword.Then(Alt<IModuleItem>(
        Seq(
            OperatorsKeyword.Then(Identifier),
            Delimited0(Either(LineComment, FunctionDeclaration), WhiteSpaces1, "methods")
                .Between(WhiteSpaces0.Then(Char('{')), LineComments.Then(WhiteSpaces0).Then(Char('}')))
            ).Map((items, start, end) => new ImplOperatorsDeclaration(items.Item1, items.Item2, start, end) as IModuleItem),
        Seq(
            Identifier,
            Delimited0(Either(LineComment, MethodDeclaration), WhiteSpaces1, "methods")
                    .Between(WhiteSpaces0.Then(Char('{')), LineComments.Then(WhiteSpaces0).Then(Char('}')))
            ).Map((items, start, end) => new ImplDeclaration(items.Item1, items.Item2, start, end) as IModuleItem)
            ));

    private static readonly Parser<ConstDeclaration> ConstDeclaration = Seq(
        DescriptionComment, Opt(PubKeyword), Tag("const").Then(WhiteSpaces1).Then(Identifier), TypeSpec,
        EqDelimiter.Then(Expression)
    ).Map((items, start, end) =>
        new ConstDeclaration(items.Item2.IsDefined, items.Item3, items.Item1, items.Item4, items.Item5,
            start, end));

    private static readonly Parser<IModuleItem> ModuleItem = Alt<IModuleItem>(
        UseNamesDeclaration.Map(item => item as IModuleItem),
        UseAliasDeclaration.Map(item => item as IModuleItem),
        FunctionDeclaration.Map(item => item as IModuleItem),
        TypeAlias.Map(item => item as IModuleItem),
        StructDeclaration.Map(item => item as IModuleItem),
        ImplDeclaration.Map(item => item as IModuleItem),
        ConstDeclaration.Map(item => item as IModuleItem),
        LineComment.Map(item => item as IModuleItem)
    );

    private static readonly Parser<List<IModuleItem>> ModuleItems =
        WhiteSpaces0.Then(DelimitedUntil(ModuleItem, WhiteSpaces1, Eof));

    public static Parser<TO2Module> Module(string moduleName) {
        return Seq(WhiteSpaces0.Then(DescriptionComment), ModuleItems)
            .Map(items => new TO2Module(moduleName, items.Item1, items.Item2));
    }
}
