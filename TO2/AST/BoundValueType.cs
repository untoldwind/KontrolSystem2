using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class BoundValueType : RealizedType {
    public readonly TO2Type elementType;

    public BoundValueType(TO2Type elementType) {
        this.elementType = elementType;
    }

    public override string Name => $"Bound<{elementType}>";

    public override string LocalName => "Bound";

    public override RealizedType UnderlyingType(ModuleContext context) =>
        new BoundValueType(elementType.UnderlyingType(context));

    public override Type GeneratedType(ModuleContext context) =>
        Type.GetType("System.Func`1")!.MakeGenericType(elementType.GeneratedType(context));
}

public class BoundValueAssignEmitter(BoundValueType boundValueType, IAssignEmitter elementAssign) : IAssignEmitter {
    public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
        var invokeMethod = boundValueType.GeneratedType(context.ModuleContext).GetMethod("Invoke") ??
                           throw new ArgumentException($"No Invoke method in generated ${boundValueType}");
        expression.EmitCode(context, false);
        context.IL.EmitCall(OpCodes.Callvirt, invokeMethod, 1);
        elementAssign.EmitConvert(context, !variable.IsConst);
        if (!dropResult) context.IL.Emit(OpCodes.Dup);
        variable.EmitStore(context);
    }

    public void EmitConvert(IBlockContext context, bool mutableTarget) {
        var invokeMethod = boundValueType.GeneratedType(context.ModuleContext).GetMethod("Invoke") ??
                           throw new ArgumentException($"No Invoke method in generated ${boundValueType}");
        context.IL.EmitCall(OpCodes.Callvirt, invokeMethod, 1);
        elementAssign.EmitConvert(context, mutableTarget);
    }
}
