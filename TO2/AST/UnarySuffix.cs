using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class UnarySuffix : Expression {
    private readonly Expression left;
    private readonly Operator op;

    public UnarySuffix(Expression left, Operator op, Position start = new(),
        Position end = new()) : base(start, end) {
        this.left = left;
        this.op = op;
    }

    public override IVariableContainer? VariableContainer {
        set => left.VariableContainer = value;
    }

    public override TypeHint? TypeHint {
        set => left.TypeHint = value;
    }

    public override TO2Type ResultType(IBlockContext context) {
        return left.ResultType(context)
            .AllowedSuffixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuiltinType.Unit)
            ?.ResultType ?? BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        left.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var leftType = left.ResultType(context);
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

        if (context.HasErrors) return;

        operatorEmitter.EmitCode(context, this);

        if (dropResult) context.IL.Emit(OpCodes.Pop);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var leftFuture = left.Eval(context);

        var operatorEmitter = leftFuture.Type.AllowedSuffixOperators(context.replModuleContext)
            .GetMatching(context.replModuleContext, op, BuiltinType.Unit);

        if (operatorEmitter == null) throw new REPLException(this, $"Suffix {op} on a {leftFuture.Type} is undefined");

        return leftFuture.Then(operatorEmitter.ResultType,
            leftResult => operatorEmitter.Eval(this, leftResult, null));
    }
}
