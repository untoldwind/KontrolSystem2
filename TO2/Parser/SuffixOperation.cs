using System.Collections.Generic;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    public interface ISuffixOperation {
    }

    public interface IAssignSuffixOperation {
    }

    public readonly struct IndexGetSuffix : ISuffixOperation, IAssignSuffixOperation {
        public readonly IndexSpec indexSpec;

        public IndexGetSuffix(IndexSpec indexSpec) => this.indexSpec = indexSpec;
    }

    public readonly struct FieldGetSuffix : ISuffixOperation, IAssignSuffixOperation {
        public readonly string fieldName;

        public FieldGetSuffix(string fieldName) => this.fieldName = fieldName;
    }

    public readonly struct MethodCallSuffix : ISuffixOperation {
        public readonly string methodName;
        public readonly List<Expression> arguments;

        public MethodCallSuffix(string methodName, List<Expression> arguments) {
            this.methodName = methodName;
            this.arguments = arguments;
        }
    }

    public readonly struct OperatorSuffix : ISuffixOperation {
        public readonly Operator op;

        public OperatorSuffix(Operator op) => this.op = op;
    }
}
