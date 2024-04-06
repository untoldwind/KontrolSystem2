using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator;

public delegate IBlockVariable? VariableResolver(string name);

public interface IBlockVariable {
    string Name { get; }
    RealizedType Type { get; }

    bool IsConst { get; }

    void EmitLoad(IBlockContext context);

    void EmitLoadPtr(IBlockContext context);

    void EmitStore(IBlockContext context);
}

public interface ITempBlockVariable : IBlockVariable, IDisposable {
}

internal class MethodParameter(string name, RealizedType type, int index, bool isConst = true) : IBlockVariable {
    private readonly int index = index;

    public string Name { get; } = name;
    public RealizedType Type { get; } = type;
    public bool IsConst { get; } = isConst;

    public void EmitLoad(IBlockContext context) {
        EmitLoadArg(context.IL, index);
    }

    public void EmitLoadPtr(IBlockContext context) {
        if (index < 256) context.IL.Emit(OpCodes.Ldarga_S, (byte)index);
        else context.IL.Emit(OpCodes.Ldarga, (short)index);
    }

    public void EmitStore(IBlockContext context) {
        if (index < 256)
            context.IL.Emit(OpCodes.Starg_S, (byte)index);
        else
            context.IL.Emit(OpCodes.Starg, (short)index);
    }

    public static void EmitLoadArg(IILEmitter il, int index) {
        switch (index) {
        case 0:
            il.Emit(OpCodes.Ldarg_0);
            return;
        case 1:
            il.Emit(OpCodes.Ldarg_1);
            return;
        case 2:
            il.Emit(OpCodes.Ldarg_2);
            return;
        case 3:
            il.Emit(OpCodes.Ldarg_3);
            return;
        case { } n when n < 256:
            il.Emit(OpCodes.Ldarg_S, (byte)index);
            return;
        default:
            il.Emit(OpCodes.Ldarg, (short)index);
            return;
        }
    }
}

internal class DeclaredVariable(string name, bool isConst, RealizedType type, ILocalRef localRef) : IBlockVariable {
    private readonly ILocalRef localRef = localRef;

    public string Name { get; } = name;
    public RealizedType Type { get; } = type;
    public bool IsConst { get; } = isConst;

    public void EmitLoad(IBlockContext context) {
        localRef.EmitLoad(context);
    }

    public void EmitLoadPtr(IBlockContext context) {
        localRef.EmitLoadPtr(context);
    }

    public void EmitStore(IBlockContext context) {
        localRef.EmitStore(context);
    }
}

public class TempVariable(RealizedType type, ITempLocalRef localRef) : ITempBlockVariable {
    private readonly ITempLocalRef localRef = localRef;

    public RealizedType Type { get; } = type;

    public string Name => "***temp***";

    public bool IsConst => false;

    public void EmitLoad(IBlockContext context) {
        localRef.EmitLoad(context);
    }

    public void EmitLoadPtr(IBlockContext context) {
        localRef.EmitLoadPtr(context);
    }

    public void EmitStore(IBlockContext context) {
        localRef.EmitStore(context);
    }

    public void Dispose() {
        localRef.Dispose();
    }
}

public class ClonedFieldVariable(RealizedType type, FieldInfo valueField) : IBlockVariable {
    public readonly FieldInfo valueField = valueField;

    public RealizedType Type { get; } = type;

    public string Name => valueField.Name;

    public bool IsConst => true;

    public void EmitLoad(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldarg_0);
        context.IL.Emit(OpCodes.Ldfld, valueField);
    }

    public void EmitLoadPtr(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldarg_0);
        context.IL.Emit(OpCodes.Ldflda, valueField);
    }

    public void EmitStore(IBlockContext context) {
    }
}
