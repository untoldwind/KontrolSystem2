using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IIndexAccessEmitter {
    TO2Type TargetType { get; }

    bool RequiresPtr { get; }

    void EmitLoad(IBlockContext context);

    void EmitPtr(IBlockContext context);

    void EmitStore(IBlockContext context, Action<IBlockContext> emitValue);

    REPLValueFuture EvalGet(Node node, REPLContext context, IREPLValue target);

    REPLValueFuture EvalAssign(Node node, REPLContext context, IREPLValue target, IREPLValue value);
}

public class InlineArrayIndexAccessEmitter : IIndexAccessEmitter {
    private readonly Expression indexExpression;
    private readonly RealizedType targetType;

    public InlineArrayIndexAccessEmitter(RealizedType targetType, Expression indexExpression) {
        this.targetType = targetType;
        this.indexExpression = indexExpression;
        this.indexExpression.TypeHint = _ => BuiltinType.Int;
    }

    public TO2Type TargetType => targetType;

    public bool RequiresPtr => false;

    public void EmitLoad(IBlockContext context) {
        var resultType = indexExpression.ResultType(context);
        if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Index has to be of type {BuiltinType.Int}",
                indexExpression.Start,
                indexExpression.End
            ));
            return;
        }

        indexExpression.EmitCode(context, false);
        BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

        context.IL.Emit(OpCodes.Conv_I4);
        if (targetType == BuiltinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
        else if (targetType == BuiltinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
        else if (targetType == BuiltinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
        else context.IL.Emit(OpCodes.Ldelem, targetType.GeneratedType(context.ModuleContext));
    }

    public void EmitPtr(IBlockContext context) {
        var resultType = indexExpression.ResultType(context);
        if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Index has to be of type {BuiltinType.Int}",
                indexExpression.Start,
                indexExpression.End
            ));
            return;
        }

        indexExpression.EmitCode(context, false);
        BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

        context.IL.Emit(OpCodes.Conv_I4);
        context.IL.Emit(OpCodes.Ldelema, targetType.GeneratedType(context.ModuleContext));
    }

    public void EmitStore(IBlockContext context, Action<IBlockContext> emitValue) {
        var resultType = indexExpression.ResultType(context);
        if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Index has to be of type {BuiltinType.Int}",
                indexExpression.Start,
                indexExpression.End
            ));
            return;
        }

        indexExpression.EmitCode(context, false);
        BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);
        context.IL.Emit(OpCodes.Conv_I4);

        emitValue(context);

        if (targetType == BuiltinType.Bool) context.IL.Emit(OpCodes.Stelem_I4);
        else if (targetType == BuiltinType.Int) context.IL.Emit(OpCodes.Stelem_I8);
        else if (targetType == BuiltinType.Float) context.IL.Emit(OpCodes.Stelem_R8);
        else context.IL.Emit(OpCodes.Stelem, targetType.GeneratedType(context.ModuleContext));
    }

    public REPLValueFuture EvalGet(Node node, REPLContext context, IREPLValue target) {
        var indexFuture = indexExpression.Eval(context);

        if (!BuiltinType.Int.IsAssignableFrom(context.replModuleContext, indexFuture.Type))
            throw new REPLException(node, $"Index has to be of type {BuiltinType.Int}");

        if (target.Type is ArrayType at && target.Value is Array a) {
            var indexAssign = BuiltinType.Int.AssignFrom(context.replModuleContext, indexFuture.Type);

            return indexFuture.Then(at.ElementType,
                i => at.ElementType.REPLCast(a.GetValue(((REPLInt)indexAssign.EvalConvert(node, i)).intValue)));
        }

        throw new REPLException(node, "Expected array");
    }

    public REPLValueFuture EvalAssign(Node node, REPLContext context, IREPLValue target, IREPLValue value) {
        var indexFuture = indexExpression.Eval(context);

        if (!BuiltinType.Int.IsAssignableFrom(context.replModuleContext, indexFuture.Type))
            throw new REPLException(node, $"Index has to be of type {BuiltinType.Int}");

        if (target.Type is ArrayType at && target.Value is Array a) {
            var indexAssign = BuiltinType.Int.AssignFrom(context.replModuleContext, indexFuture.Type);

            return indexFuture.Then(at.ElementType, i => {
                a.SetValue(value.Value, ((REPLInt)indexAssign.EvalConvert(node, i)).intValue);
                return at.ElementType.REPLCast(value.Value);
            });
        }

        throw new REPLException(node, "Expected array");
    }
}
