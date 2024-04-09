using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class BoundArrayLikeIndexAccess(Type implementationType, Expression indexExpression) : IIndexAccessEmitter {
    private readonly MethodInfo getElementMethod = implementationType.GetMethod("GetElement")!;
    private readonly Type elementType = implementationType.GetInterface("IArrayLike`1").GenericTypeArguments[0];

    public TO2Type TargetType => BindingGenerator.MapNativeType(elementType);

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
        BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context, false);

        context.IL.EmitCall(OpCodes.Callvirt, getElementMethod, 2);
    }

    public void EmitPtr(IBlockContext context) {
        EmitLoad(context);

        var tempVariable = context.MakeTempVariable(BindingGenerator.MapNativeType(elementType));
        tempVariable.EmitStore(context);
        tempVariable.EmitLoadPtr(context);
    }

    public void EmitStore(IBlockContext context, Action<IBlockContext> emitValue) {
        context.AddError(new StructuralError(
            StructuralError.ErrorType.IncompatibleTypes,
            $"Elements of ${BindingGenerator.MapNativeType(elementType)} can not be updated",
            indexExpression.Start,
            indexExpression.End
        ));
    }
}
