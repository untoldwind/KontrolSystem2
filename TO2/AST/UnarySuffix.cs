using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class UnarySuffix(Expression left, Operator op, Position start = new(),
    Position end = new()) : Expression(start, end) {
    private readonly Expression left = left;
    private readonly Operator op = op;

    public override IVariableContainer? VariableContainer {
        set => left.VariableContainer = value;
    }

    public override TypeHint? TypeHint {
        set => left.TypeHint = value;
    }

    public override TO2Type ResultType(IBlockContext context) {
        var leftType = left.ResultType(context);

        if (leftType is BoundValueType bound)
            leftType = bound.elementType;

        return leftType
            .AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuiltinType.Unit)
            ?.ResultType ?? BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        left.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var leftType = left.ResultType(context);
        IAssignEmitter? leftConvert = null;

        if (leftType is BoundValueType bound) {
            leftType = bound.elementType;
            leftConvert = bound.elementType.AssignFrom(context.ModuleContext, bound);
        }

        var operatorEmitter = leftType.AllowedSuffixOperators(context.ModuleContext)
            .GetMatching(context.ModuleContext, op, BuiltinType.Unit);

        if (context.HasErrors) return;

        if (operatorEmitter == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidOperator,
                $"Suffix {op} on a {leftType} is undefined",
                Start,
                End
            ));
            return;
        }

        left.EmitCode(context, false);
        leftConvert?.EmitConvert(context, false);

        if (context.HasErrors) return;

        operatorEmitter.EmitCode(context, this);

        if (dropResult) context.IL.Emit(OpCodes.Pop);
    }

    public override string ToString() => $"{left} {op.ToPrettyString()}";
}
