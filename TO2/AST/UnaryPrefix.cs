using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class UnaryPrefix : Expression {
    private readonly Operator op;
    private readonly Expression right;

    public UnaryPrefix(Operator op, Expression right, Position start = new(),
        Position end = new()) : base(start, end) {
        this.op = op;
        this.right = right;
    }

    public override IVariableContainer? VariableContainer {
        set => right.VariableContainer = value;
    }

    public override TypeHint? TypeHint {
        set => right.TypeHint = value;
    }

    public override TO2Type ResultType(IBlockContext context) {
        return right.ResultType(context)
            .AllowedPrefixOperators(context.ModuleContext).GetMatching(context.ModuleContext, op, BuiltinType.Unit)
            ?.ResultType ?? BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        right.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var rightType = right.ResultType(context);
        var operatorEmitter = rightType.AllowedPrefixOperators(context.ModuleContext)
            .GetMatching(context.ModuleContext, op, BuiltinType.Unit);

        if (context.HasErrors) return;

        if (operatorEmitter == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidOperator,
                $"Prefix {op} on a {rightType} is undefined",
                Start,
                End
            ));
            return;
        }

        right.EmitCode(context, false);

        if (context.HasErrors) return;

        operatorEmitter.EmitCode(context, this);

        if (dropResult) context.IL.Emit(OpCodes.Pop);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var rightFuture = right.Eval(context);

        var operatorEmitter = rightFuture.Type.AllowedPrefixOperators(context.replModuleContext)
            .GetMatching(context.replModuleContext, op, BuiltinType.Unit);

        if (operatorEmitter == null) throw new REPLException(this, $"Prefix {op} on a {rightFuture.Type} is undefined");

        return rightFuture.Then(operatorEmitter.ResultType,
            rightResult => operatorEmitter.Eval(this, rightResult, null));
    }
}
