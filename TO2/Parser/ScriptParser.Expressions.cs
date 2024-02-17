using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser;

using static Parsers;
using static TO2ParserCommon;
using static TO2ParserLiterals;

public static class TO2ParserExpressions {
    public static readonly Parser<Expression> Expression = ExpressionImpl;

    private static readonly Parser<bool> LetOrConst = Alt(LetKeyword.To(false), ConstKeyword.To(true));

    public static readonly Parser<IBlockItem> VariableDeclaration = Seq(
        LetOrConst, Alt(
            DeclarationParameter.Map(item => (true, new List<DeclarationParameter> { item })),
            Delimited1(DeclarationParameterOrPlaceholder, CommaDelimiter)
                .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
        ), EqDelimiter.Then(Expression)
    ).Map((items, start, end) => {
        if (items.Item2.Item1)
            return new VariableDeclaration(items.Item2.Item2[0], items.Item1, items.Item3, start, end);
        return new TupleDeconstructDeclaration(items.Item2.Item2, items.Item1, items.Item3, start, end) as
            IBlockItem;
    });

    private static readonly Parser<Expression> ReturnExpression = Seq(
        Tag("return"), Opt(Spacing0.Then(Expression))
    ).Map((items, start, end) => {
        if (items.Item2.IsDefined) return new ReturnValue(items.Item2.Value, start, end);
        return new ReturnEmpty(start, end) as Expression;
    });

    internal static readonly Parser<Expression> WhileExpression = Seq(
        Tag("while").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(Expression),
        WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Expression)
    ).Map((items, start, end) => new While(items.Item1, items.Item2, start, end));

    internal static readonly Parser<Expression> ForInExpression = Seq(
        Tag("for").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(Alt(
            DeclarationParameter.Map(item => (true, new List<DeclarationParameter> { item })),
            Delimited1(DeclarationParameterOrPlaceholder, CommaDelimiter)
                .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
        )),
        WhiteSpaces1.Then(Tag("in")).Then(WhiteSpaces1).Then(Expression),
        WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Expression)
    ).Map((items, start, end) => {
        if (items.Item1.Item1)
            return new ForIn(items.Item1.Item2[0].target!, items.Item1.Item2[0].type, items.Item2, items.Item3,
                start, end);
        return new ForInDeconstruct(items.Item1.Item2, items.Item2, items.Item3, start, end) as Expression;
    });

    private static readonly Parser<Expression> BreakExpression =
        Tag("break").Map((_, start, end) => new Break(start, end));

    private static readonly Parser<Expression> ContinueExpression =
        Tag("continue").Map((_, start, end) => new Continue(start, end));

    private static readonly Parser<Expression> Block = Char('{').Then(WhiteSpaces0).Then(DelimitedUntil(
        Alt(
            Expression,
            LineComment,
            VariableDeclaration,
            ReturnExpression,
            WhileExpression,
            ForInExpression,
            BreakExpression,
            ContinueExpression
        ), WhiteSpaces1, Char('}'))).Map(expressions => new Block(expressions));

    private static readonly Parser<List<Expression>> CallArguments = Char('(').Then(WhiteSpaces0)
        .Then(DelimitedUntil(Expression, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

    private static readonly Parser<Expression> VariableRefOrCall = Seq(
        IdentifierPath, Opt(Spacing0.Then(CallArguments))
    ).Map((items, start, end) => {
        var fullName = items.Item1;
        if (items.Item2.IsDefined) return new Call(fullName, items.Item2.Value, start, end);

        return new VariableGet(fullName, start, end) as Expression;
    });

    private static readonly Parser<Expression> TupleCreate =
        DelimitedN_M(2, null, Expression, CommaDelimiter, "<expression>")
            .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map((expressions, start, end) =>
                new TupleCreate(expressions, start, end));

    private static readonly Parser<Expression> RecordCreate = Seq(
        Opt(TypeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
        Delimited1(
            Seq(Identifier, Spacing0.Then(Char(':')).Then(Spacing0).Then(Expression)),
            CommaDelimiter, "<expression>").Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))
    ).Map((items, start, end) =>
        new RecordCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

    private static readonly Parser<Expression> ArrayCreate = Seq(
        Opt(TypeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
        Char('[').Then(WhiteSpaces0).Then(DelimitedUntil(Expression, CommaDelimiter, WhiteSpaces0.Then(Char(']'))))
    ).Map((items, start, end) =>
        new ArrayCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

    private static readonly Parser<FunctionParameter> LambdaParameter = Seq(
        Identifier, Opt(TypeSpec)
    ).Map(param => new FunctionParameter(param.Item1, param.Item2.IsDefined ? param.Item2.Value : null, null));

    private static readonly Parser<List<FunctionParameter>> LambdaParameters = Char('(').Then(WhiteSpaces0)
        .Then(DelimitedUntil(LambdaParameter, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

    private static readonly Parser<Expression> Lambda = Seq(
        Tag("fn").Then(Spacing0).Then(LambdaParameters),
        WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(Expression)
    ).Map((items, start, end) => new Lambda(items.Item1, items.Item2, start, end));

    private static readonly Parser<Expression> BracketTerm = Expression
        .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))
        .Map((expression, start, end) => new Bracket(expression, start, end));

    private static readonly Parser<Expression> Term = Alt(
        LiteralBool,
        LiteralFloat,
        LiteralInt,
        LiteralString,
        BracketTerm,
        Block,
        ArrayCreate,
        TupleCreate,
        RecordCreate,
        VariableRefOrCall,
        Lambda
    );

    private static readonly Parser<IndexSpec> IndexSpec = Expression.Map(expression => new IndexSpec(expression));

    private static readonly Parser<Operator> SuffixOp = Char('?').Map(_ => Operator.Unwrap);

    private static readonly Parser<ISuffixOperation> SuffixOps = Alt(
        Seq(WhiteSpaces0.Then(Char('.')).Then(WhiteSpaces0).Then(Identifier), Opt(Spacing0.Then(CallArguments)))
            .Map(item => {
                if (item.Item2.IsDefined) return new MethodCallSuffix(item.Item1, item.Item2.Value);
                return new FieldGetSuffix(item.Item1) as ISuffixOperation;
            }),
        Spacing0.Then(IndexSpec.Between(Char('[').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(']'))))
            .Map(indexSpec => new IndexGetSuffix(indexSpec) as ISuffixOperation),
        Spacing0.Then(SuffixOp).Map(op => new OperatorSuffix(op) as ISuffixOperation)
    );

    private static readonly Parser<Expression> TermWithSuffixOps = Terminated(Term.Fold0(
        Terminated(SuffixOps, LineComments),
        (target, suffixOp, start, end) => suffixOp.GetExpression(target, start, end)), LineComments);

    private static readonly Parser<Operator> UnaryPrefixOp = Alt(
        Char('-').To(Operator.Neg),
        Char('!').To(Operator.Not),
        Char('~').To(Operator.BitNot)
    );

    private static readonly Parser<Expression> UnaryPrefixExpr = Alt(
        Seq(UnaryPrefixOp, WhiteSpaces0.Then(LineComments).Then(TermWithSuffixOps)).Map((items, start, end) =>
            new UnaryPrefix(items.Item1, items.Item2, start, end)),
        TermWithSuffixOps
    );

    private static readonly Parser<Operator> PowBinaryOp = Alt(
        Tag("**").To(Operator.Pow)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> PowExpr = Chain(UnaryPrefixExpr, PowBinaryOp,
        (left, op, right, start, end) => new Binary(left, op, right, start, end));

    private static readonly Parser<Operator> MulDivBinaryOp = Alt(
        Char('*').To(Operator.Mul),
        Char('/').To(Operator.Div),
        Char('%').To(Operator.Mod)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> MulDivBinaryExpr = Chain(PowExpr, MulDivBinaryOp,
        (left, op, right, start, end) => new Binary(left, op, right, start, end));

    private static readonly Parser<Operator> AddSubBinaryOp = Alt(
        Char('+').To(Operator.Add),
        Char('-').To(Operator.Sub)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> AddSubBinaryExpr = Chain(MulDivBinaryExpr, AddSubBinaryOp,
        (left, op, right, start, end) => new Binary(left, op, right, start, end));

    private static readonly Parser<Operator> BITOp = Alt(
        Char('&').To(Operator.BitAnd),
        Char('|').To(Operator.BitOr),
        Char('^').To(Operator.BitXor)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> BITBinaryExpr = Chain(AddSubBinaryExpr, BITOp,
        (left, op, right, start, end) => new Binary(left, op, right, start, end));

    private static readonly Parser<Expression> RangeCreate = Seq(
        BITBinaryExpr, Opt(Seq(Spacing0.Then(Tag("..")).Then(Opt(Char('.'))),
            Spacing0.Then(BITBinaryExpr)))
    ).Map((items, start, end) => {
        if (items.Item2.IsDefined)
            return new RangeCreate(items.Item1, items.Item2.Value.Item2, items.Item2.Value.Item1.IsDefined, start, end);
        return items.Item1;
    });

    private static readonly Parser<Expression> UnapplyExpr = Seq(
        Identifier,
        Spacing0.Then(
            Delimited0(Identifier, CommaDelimiter)
                .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))),
        Spacing0.Then(Char('=')).Then(Spacing0).Then(BITBinaryExpr)
    ).Map((items, start, end) => new Unapply(items.Item1, items.Item2, items.Item3, start, end));

    private static readonly Parser<Operator> CompareOp = Alt(
        Tag("==").To(Operator.Eq),
        Tag("!=").To(Operator.NotEq),
        Tag("<=").To(Operator.Le),
        Tag(">=").To(Operator.Ge),
        Char('<').To(Operator.Lt),
        Char('>').To(Operator.Gt)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> CompareExpr = Chain(Alt(UnapplyExpr, RangeCreate), CompareOp,
        (left, op, right, start, end) => new Binary(left, op, right, start, end));

    private static readonly Parser<Operator> BooleanOp = Alt(
        Tag("&&").To(Operator.BoolAnd),
        Tag("||").To(Operator.BoolOr)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<Expression> BooleanExpr = Chain(CompareExpr, BooleanOp,
        (left, op, right, start, end) => new BinaryBool(left, op, right, start, end));

    private static readonly Parser<Expression> IfBody = Alt(Expression, ReturnExpression, BreakExpression,
        ContinueExpression).Between(LineComments.Then(WhiteSpaces0), LineComments);

    private static readonly Parser<Expression> IfExpr = Seq(
        Tag("if").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(BooleanExpr),
        WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(IfBody),
        Opt(WhiteSpaces1.Then(Tag("else")).Then(WhiteSpaces1).Then(IfBody))
    ).Map((items, start, end) => {
        if (items.Item3.IsDefined)
            return new IfThenElse(items.Item1, items.Item2, items.Item3.Value, start, end);
        return new IfThen(items.Item1, items.Item2, start, end) as Expression;
    });

    private static readonly Parser<Operator> AssignOp = Alt(
        Tag("=").To(Operator.Assign),
        Tag("+=").To(Operator.AddAssign),
        Tag("-=").To(Operator.SubAssign),
        Tag("*=").To(Operator.MulAssign),
        Tag("/=").To(Operator.DivAssign),
        Tag("%=").To(Operator.ModAssign),
        Tag("|=").To(Operator.BitOrAssign),
        Tag("&=").To(Operator.BitAndAssign),
        Tag("&=").To(Operator.BitXorAssign),
        Tag("**=").To(Operator.PowAssign)
    ).Between(WhiteSpaces0, WhiteSpaces0);

    private static readonly Parser<IAssignSuffixOperation> AssignSuffixOps = Alt(
        WhiteSpaces0.Then(Char('.')).Then(WhiteSpaces0).Then(Identifier)
            .Map(field => new FieldGetSuffix(field) as IAssignSuffixOperation),
        Spacing0.Then(IndexSpec.Between(Char('[').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(']'))))
            .Map(indexSpec => new IndexGetSuffix(indexSpec) as IAssignSuffixOperation)
    );

    private static readonly Parser<Expression> Assignment = Seq(
        Identifier, Many0(AssignSuffixOps), AssignOp, Alt(BooleanExpr, IfExpr)
    ).Map((items, start, end) => {
        var suffixCount = items.Item2.Count;
        if (suffixCount == 0)
            return new VariableAssign(items.Item1, items.Item3, items.Item4, start, end);
        var last = items.Item2[suffixCount - 1];
        var target = items.Item2.Take(suffixCount - 1)
            .Aggregate(new VariableGet(new List<string> { items.Item1 }, start, end) as Expression,
                (result, op) => op.GetExpression(result, start, end));
        return last.AssignExpression(target, items.Item3, items.Item4, start, end);
    });

    private static readonly Parser<List<(string source, string target)>> SourceTargetList = Delimited1(Alt(
        Seq(Identifier, Spacing0.Then(Char('@')).Then(Spacing0).Then(Identifier)),
        Char('_').To(("", "")),
        Identifier.Map(id => (id, id))
    ), CommaDelimiter).Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')));

    private static readonly Parser<Expression> TupleDeconstructAssignment = Seq(
        SourceTargetList, EqDelimiter.Then(Alt(BooleanExpr, IfExpr))
    ).Map((items, start, end) => new TupleDeconstructAssign(items.Item1, items.Item2, start, end));

    private static readonly Parser<Expression> TopLevelExpression = Alt(
        TupleDeconstructAssignment,
        Assignment,
        IfExpr,
        BooleanExpr
    );

    private static IResult<Expression> ExpressionImpl(IInput input) {
        return TopLevelExpression(input);
    }
}
