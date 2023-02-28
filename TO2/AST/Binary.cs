using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class Binary : Expression {
        private readonly Operator op;
        private readonly Expression left;
        private readonly Expression right;

        public Binary(Expression left, Operator op, Expression right, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override IVariableContainer VariableContainer {
            set {
                left.VariableContainer = value;
                right.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            IOperatorEmitter operatorEmitter =
                leftType.AllowedSuffixOperators(context.ModuleContext)
                    .GetMatching(context.ModuleContext, op, rightType) ??
                rightType.AllowedPrefixOperators(context.ModuleContext)
                    .GetMatching(context.ModuleContext, op, leftType);

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Cannot {op} a {leftType} with a {rightType}",
                    Start,
                    End
                ));
                return BuiltinType.Unit;
            }

            return operatorEmitter.ResultType;
        }

        public override void Prepare(IBlockContext context) {
            left.Prepare(context);
            right.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            if (context.HasErrors) return;

            IOperatorEmitter leftEmitter = leftType.AllowedSuffixOperators(context.ModuleContext)
                .GetMatching(context.ModuleContext, op, rightType);
            IOperatorEmitter rightEmitter = rightType.AllowedPrefixOperators(context.ModuleContext)
                .GetMatching(context.ModuleContext, op, leftType);

            if (leftEmitter == null && rightEmitter == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Cannot {op} a {leftType} with a {rightType}",
                    Start,
                    End
                ));
                return;
            }

            right.Prepare(context);

            left.EmitCode(context, false);
            rightEmitter?.OtherType.AssignFrom(context.ModuleContext, leftType).EmitConvert(context);
            right.EmitCode(context, false);
            leftEmitter?.OtherType.AssignFrom(context.ModuleContext, rightType).EmitConvert(context);

            if (context.HasErrors) return;

            if (leftEmitter != null) leftEmitter.EmitCode(context, this);
            else rightEmitter.EmitCode(context, this);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }
    }
}
