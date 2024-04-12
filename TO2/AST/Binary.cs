using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class Binary(
    Expression left,
    Operator op,
    Expression right,
    Position start = new(),
    Position end = new())
    : Expression(start, end) {
    public override IVariableContainer? VariableContainer {
        set {
            left.VariableContainer = value;
            right.VariableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) {
        var leftType = left.ResultType(context);
        var rightType = right.ResultType(context);

        if (leftType is BoundValueType boundLeft)
            leftType = boundLeft.elementType;

        var operatorEmitter =
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
        var leftType = left.ResultType(context);
        IAssignEmitter? leftConvert = null;
        var rightType = right.ResultType(context);

        if (leftType is BoundValueType boundLeft) {
            leftType = boundLeft.elementType;
            leftConvert = boundLeft.elementType.AssignFrom(context.ModuleContext, boundLeft);
        }

        if (context.HasErrors) return;

        var leftEmitter = leftType.AllowedSuffixOperators(context.ModuleContext)
            .GetMatching(context.ModuleContext, op, rightType);
        var rightEmitter = rightType.AllowedPrefixOperators(context.ModuleContext)
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

        try {
            right.Prepare(context);

            left.EmitCode(context, false);
            if (rightEmitter != null)
                rightEmitter.OtherType.AssignFrom(context.ModuleContext, leftType).EmitConvert(context, false);
            else
                leftConvert?.EmitConvert(context, false);

            right.EmitCode(context, false);
            leftEmitter?.OtherType.AssignFrom(context.ModuleContext, rightType).EmitConvert(context, false);

            if (context.HasErrors) return;

            if (leftEmitter != null) leftEmitter.EmitCode(context, this);
            else rightEmitter!.EmitCode(context, this);

            if (dropResult) context.IL.Emit(OpCodes.Pop);
        } catch (CodeGenerationException e) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.CoreGeneration,
                e.Message,
                Start,
                End
            ));
        }
    }

    public override string ToString() => $"({left} {op.ToPrettyString()} {right})";
}
