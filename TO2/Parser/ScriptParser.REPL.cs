
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Parser {
    using static Parsers;
    using static TO2ParserCommon;
    using static TO2ParserExpressions;
    using static TO2ParserFunctions;
    using static TO2ParserModule;

    public static class TO2ParserREPL {
        private static readonly Parser<Node> REPLItem = Opt(LineComment).Then(Alt<Node>(
            UseNamesDeclaration,
            UseAliasDeclaration,
            TypeAlias,
            Expression,
            WhileExpression,
            ForInExpression,
            VariableDeclaration.Map(item => item as Node)
        ));

        public static readonly Parser<List<Node>> REPLItems =
            WhiteSpaces0.Then(DelimitedUntil(REPLItem, WhiteSpaces1, Eof));
    }
}
