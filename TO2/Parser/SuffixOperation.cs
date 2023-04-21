using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    public interface ISuffixOperation {
        Expression GetExpression(Expression target, Position start, Position end);
    }

    public interface IAssignSuffixOperation : ISuffixOperation {
        Expression AssignExpression(Expression target, Operator op, Expression value, Position start, Position end);
    }

    public readonly struct IndexGetSuffix : ISuffixOperation, IAssignSuffixOperation {
        public readonly IndexSpec indexSpec;

        public IndexGetSuffix(IndexSpec indexSpec) => this.indexSpec = indexSpec;
        public Expression GetExpression(Expression target, Position start, Position end) =>
            new IndexGet(target, indexSpec, start, end);

        public Expression AssignExpression(Expression target, Operator op, Expression value, Position start, Position end) =>
            new IndexAssign(target, indexSpec, op, value, start, end);
    }

    public readonly struct FieldGetSuffix : ISuffixOperation, IAssignSuffixOperation {
        public readonly string fieldName;

        public FieldGetSuffix(string fieldName) => this.fieldName = fieldName;
        public Expression GetExpression(Expression target, Position start, Position end) =>
            new FieldGet(target, fieldName, start, end);

        public Expression AssignExpression(Expression target, Operator op, Expression value, Position start,
            Position end) =>
            new FieldAssign(target, fieldName, op, value, start, end);
    }

    public readonly struct MethodCallSuffix : ISuffixOperation {
        public readonly string methodName;
        public readonly List<Expression> arguments;

        public MethodCallSuffix(string methodName, List<Expression> arguments) {
            this.methodName = methodName;
            this.arguments = arguments;
        }

        public Expression GetExpression(Expression target, Position start, Position end) =>
            new MethodCall(target, methodName, arguments, start, end);
    }

    public readonly struct OperatorSuffix : ISuffixOperation {
        public readonly Operator op;

        public OperatorSuffix(Operator op) => this.op = op;
        public Expression GetExpression(Expression target, Position start, Position end) =>
            new UnarySuffix(target, op, start, end);
    }
}
