using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser;

public interface ISuffixOperation {
    Expression GetExpression(Expression target, Position start, Position end);
}

public interface IAssignSuffixOperation : ISuffixOperation {
    Expression AssignExpression(Expression target, Operator op, Expression value, Position start, Position end);
}

public readonly struct IndexGetSuffix(IndexSpec indexSpec) : ISuffixOperation, IAssignSuffixOperation {
    public readonly IndexSpec indexSpec = indexSpec;

    public Expression GetExpression(Expression target, Position start, Position end) {
        return new IndexGet(target, indexSpec, start, end);
    }

    public Expression AssignExpression(Expression target, Operator op, Expression value, Position start, Position end) {
        return new IndexAssign(target, indexSpec, op, value, start, end);
    }
}

public readonly struct FieldGetSuffix(string fieldName) : ISuffixOperation, IAssignSuffixOperation {
    public readonly string fieldName = fieldName;

    public Expression GetExpression(Expression target, Position start, Position end) {
        return new FieldGet(target, fieldName, start, end);
    }

    public Expression AssignExpression(Expression target, Operator op, Expression value, Position start,
        Position end) {
        return new FieldAssign(target, fieldName, op, value, start, end);
    }
}

public readonly struct MethodCallSuffix(string methodName, List<Expression> arguments) : ISuffixOperation {
    public readonly string methodName = methodName;
    public readonly List<Expression> arguments = arguments;

    public Expression GetExpression(Expression target, Position start, Position end) {
        return new MethodCall(target, methodName, arguments, start, end);
    }
}

public readonly struct OperatorSuffix(Operator op) : ISuffixOperation {
    public readonly Operator op = op;

    public Expression GetExpression(Expression target, Position start, Position end) {
        return new UnarySuffix(target, op, start, end);
    }
}
