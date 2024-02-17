using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class Break : Expression {
    public Break(Position start = new(), Position end = new()) : base(start, end) {
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!context.InnerLoop.HasValue) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidScope,
                "break can only be used inside a loop",
                Start,
                End
            ));
            return;
        }

        context.IL.Emit(context.InnerLoop.Value.end.isShort ? OpCodes.Br_S : OpCodes.Br,
            context.InnerLoop.Value.end);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(REPLBreak.INSTANCE);
    }
}

public class Continue : Expression {
    public Continue(Position start = new(), Position end = new()) : base(start, end) {
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!context.InnerLoop.HasValue) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidScope,
                "continue can only be used inside a loop",
                Start,
                End
            ));
            return;
        }

        context.IL.Emit(context.InnerLoop.Value.start.isShort ? OpCodes.Br_S : OpCodes.Br,
            context.InnerLoop.Value.start);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(REPLContinue.INSTANCE);
    }
}
