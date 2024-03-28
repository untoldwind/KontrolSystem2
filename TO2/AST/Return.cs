using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ReturnEmpty(Position start = new(), Position end = new()) : Expression(start, end) {
    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (context.ExpectedReturn != BuiltinType.Unit) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Expected a return value of type {context.ExpectedReturn}",
                Start,
                End
            ));
            return;
        }

        context.IL.Emit(OpCodes.Ldnull);
        if (context.IsAsync)
            context.IL.EmitNew(OpCodes.Newobj,
                context.MethodBuilder!.ReturnType.GetConstructor([typeof(object)])!);

        ILChunks.GenerateFunctionLeave(context);
        context.IL.EmitReturn(context.MethodBuilder!.ReturnType);
        if (!dropResult)
            context.IL.Emit(OpCodes.Ldnull);
    }

    internal override Expression CollapseFinalReturn() => new Block([], Start, End);

    public override REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(new REPLReturn(REPLUnit.INSTANCE));
}

public class ReturnValue : Expression {
    internal readonly Expression returnValue;

    public ReturnValue(Expression returnValue, Position start = new(), Position end = new()) :
        base(start, end) {
        this.returnValue = returnValue;
        this.returnValue.TypeHint = context => context.ExpectedReturn.UnderlyingType(context.ModuleContext);
    }

    public override IVariableContainer? VariableContainer {
        set => returnValue.VariableContainer = value;
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var returnType = returnValue.ResultType(context);

        if (!context.ExpectedReturn.IsAssignableFrom(context.ModuleContext, returnType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Expected a return value of type {context.ExpectedReturn}, but got {returnType}",
                Start,
                End
            ));
            return;
        }

        returnValue.EmitCode(context, false);

        if (context.HasErrors) return;

        context.ExpectedReturn.AssignFrom(context.ModuleContext, returnType).EmitConvert(context, false);
        if (context.IsAsync)
            context.IL.EmitNew(OpCodes.Newobj,
                context.MethodBuilder!.ReturnType.GetConstructor([returnType.GeneratedType(context.ModuleContext)])!);

        ILChunks.GenerateFunctionLeave(context);
        context.IL.EmitReturn(context.MethodBuilder!.ReturnType);
        if (!dropResult)
            context.IL.Emit(OpCodes.Ldnull);
    }

    internal override Expression CollapseFinalReturn() => returnValue;

    public override REPLValueFuture Eval(REPLContext context) => returnValue.Eval(context).Then(BuiltinType.Unit, value => new REPLReturn(value));
}
