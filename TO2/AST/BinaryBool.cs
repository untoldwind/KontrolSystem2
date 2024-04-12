using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class BinaryBool : Expression {
    private readonly Expression left;
    private readonly Operator op;
    private readonly Expression right;

    public BinaryBool(Expression left, Operator op, Expression right, Position start = new(),
        Position end = new()) : base(start, end) {
        this.left = left;
        this.op = op;
        this.right = right;
        this.left.TypeHint = _ => BuiltinType.Bool;
        this.right.TypeHint = _ => BuiltinType.Bool;
    }

    public override IVariableContainer? VariableContainer {
        set {
            left.VariableContainer = value;
            right.VariableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Bool;

    public override void Prepare(IBlockContext context) {
        left.Prepare(context);
        right.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var leftType = left.ResultType(context);
        IAssignEmitter? leftConvert = null;
        var rightType = right.ResultType(context);
        IAssignEmitter? rightConvert = null;

        if (leftType is BoundValueType boundLeft) {
            leftType = boundLeft.elementType;
            leftConvert = boundLeft.elementType.AssignFrom(context.ModuleContext, boundLeft);
        }

        if (rightType is BoundValueType boundRight) {
            rightType = boundRight.elementType;
            rightConvert = boundRight.elementType.AssignFrom(context.ModuleContext, boundRight);
        }

        if (leftType != BuiltinType.Bool)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                "Expected boolean",
                left.Start,
                left.End
            ));
        if (rightType != BuiltinType.Bool)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                "Expected boolean",
                right.Start,
                right.End
            ));

        if (context.HasErrors) return;

        left.EmitCode(context, false);
        leftConvert?.EmitConvert(context, false);
        if (!dropResult) context.IL.Emit(OpCodes.Dup);

        var rightCount = right.GetILCount(context, dropResult);
        if (rightConvert != null) {
            var countingContext = context.CloneCountingContext();
            rightConvert.EmitConvert(countingContext, false);
            rightCount.opCodes += context.IL.ILSize;
            rightCount.stack += context.IL.StackCount;
        }
        var skipRight = context.IL.DefineLabel(rightCount.opCodes < 124);

        if (context.HasErrors) return;

        switch (op) {
        case Operator.BoolAnd:
            context.IL.Emit(skipRight.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipRight);
            break;
        case Operator.BoolOr:
            context.IL.Emit(skipRight.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, skipRight);
            break;
        default:
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidOperator,
                $"Invalid boolean operator {op}",
                Start,
                End
            ));
            return;
        }

        if (!dropResult) context.IL.Emit(OpCodes.Pop);

        right.EmitCode(context, dropResult);
        rightConvert?.EmitConvert(context, false);
        context.IL.MarkLabel(skipRight);
    }

    public override string ToString() => $"({left} {op.ToPrettyString()} {right})";
}
