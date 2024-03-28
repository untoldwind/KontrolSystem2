using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class BoundEnumerableForInSource(Type implementationType) : IForInSource {
    private readonly MethodInfo getEnumeratorMethod = implementationType.GetMethod("GetEnumerator")!;
    private readonly Type elementType = implementationType.GetInterface("IEnumerable`1").GenericTypeArguments[0];


    private ILocalRef? enumeratorRef;

    public RealizedType ElementType => BindingGenerator.MapNativeType(elementType);

    public void EmitInitialize(IBlockContext context) {
        context.IL.EmitCall(OpCodes.Callvirt, getEnumeratorMethod, 1);
        enumeratorRef = context.DeclareHiddenLocal(getEnumeratorMethod.ReturnType);
        enumeratorRef.EmitStore(context);
    }

    public void EmitCheckDone(IBlockContext context, LabelRef loop) {
        enumeratorRef!.EmitLoad(context);
        context.IL.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext")!, 1);
        context.IL.Emit(loop.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, loop);
    }

    public void EmitNext(IBlockContext context) {
        enumeratorRef!.EmitLoad(context);
        context.IL.EmitCall(OpCodes.Callvirt, getEnumeratorMethod.ReturnType.GetProperty("Current")!.GetMethod, 1);
    }
}
