using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator;

public class LocalBuilderRef : ILocalRef {
    internal readonly LocalBuilder localBuilder;

    public LocalBuilderRef(LocalBuilder localBuilder) {
        this.localBuilder = localBuilder;
    }

    public int LocalIndex => localBuilder.LocalIndex;

    public Type LocalType => localBuilder.LocalType;
}

public class TempLocalBuilderRef : LocalBuilderRef, ITempLocalRef {
    private readonly Action<TempLocalBuilderRef> onDispose;

    public TempLocalBuilderRef(LocalBuilder localBuilder, Action<TempLocalBuilderRef> onDispose) : base(
        localBuilder) {
        this.onDispose = onDispose;
    }

    public void Dispose() {
        onDispose(this);
    }
}

public class GeneratorILEmitter : IILEmitter {
    private readonly ILGenerator generator;
    private readonly Dictionary<Type, Queue<ITempLocalRef>> tempLocals;
    private int scopeCount;

    public GeneratorILEmitter(ILGenerator generator) {
        this.generator = generator;
        StackCount = 0;
        scopeCount = 0;
        tempLocals = new Dictionary<Type, Queue<ITempLocalRef>>();
    }

    public void Emit(OpCode opCode) {
        generator.Emit(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, byte arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, short arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, int arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, long arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, double arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, string arg) {
        generator.Emit(opCode, arg);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, ILocalRef localRef) {
        generator.Emit(opCode, ((LocalBuilderRef)localRef).localBuilder);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, LabelRef labelRef) {
        generator.Emit(opCode, labelRef.label);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, IEnumerable<LabelRef> labels) {
        generator.Emit(opCode, labels.Select(l => l.label).ToArray());
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, FieldInfo field) {
        generator.Emit(opCode, field);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null) {
        generator.Emit(opCode, type);
        AdjustStack(opCode, argumentCount, resultCount);
    }

    public void EmitPtr(OpCode opCode, MethodInfo methodInfo) {
        generator.Emit(opCode, methodInfo);
        AdjustStack(opCode);
    }

    public void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null,
        int? resultCount = null) {
        generator.Emit(opCode, constructor);
        AdjustStack(opCode, argumentCount ?? constructor.GetParameters().Length, resultCount);
    }

    public void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount) {
        generator.Emit(opCode, methodInfo);
        AdjustStack(opCode, argumentCount, methodInfo.ReturnType != typeof(void) ? 1 : 0);
    }

    public ILocalRef DeclareLocal(Type type) {
        var localBuilder = generator.DeclareLocal(type);
        LastLocalIndex = localBuilder.LocalIndex;
        return new LocalBuilderRef(localBuilder);
    }

    public ITempLocalRef TempLocal(Type type) {
        if (tempLocals.ContainsKey(type)) {
            if (tempLocals[type].Count > 0) return tempLocals[type].Dequeue();
        } else {
            tempLocals[type] = new Queue<ITempLocalRef>();
        }

        var localBuilder = generator.DeclareLocal(type);
        LastLocalIndex = localBuilder.LocalIndex;
        return new TempLocalBuilderRef(localBuilder, tempLocals[type].Enqueue);
    }

    public LabelRef DefineLabel(bool isShort) {
        return new LabelRef(generator.DefineLabel(), isShort);
    }


    public void MarkLabel(LabelRef labelRef) {
        generator.MarkLabel(labelRef.label);
    }

    public void BeginScope() {
        generator.BeginScope();
        scopeCount++;
    }

    public void EndScope() {
        generator.EndScope();
        scopeCount--;
        if (scopeCount < 0) throw new CodeGenerationException("Negative scope count");
    }

    public void EmitReturn(Type returnType) {
        if (returnType == typeof(void)) {
            for (var i = StackCount; i > 0; i--) Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ret);
            AdjustStack(OpCodes.Ret, 0);
        } else {
            for (var i = StackCount - 1; i > 0; i--) Emit(OpCodes.Pop);
            if (StackCount == 0) Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ret);
            AdjustStack(OpCodes.Ret, 1);
        }
    }

    public int LastLocalIndex { get; private set; }

    public int ILSize => generator.ILOffset;

    public int StackCount { get; private set; }

    public void AdjustStack(int diff) {
        StackCount += diff;
    }

    private void AdjustStack(OpCode opCode, int? varPop = null, int? varPush = null) {
        StackCount += InstructionSize.StackDiff(opCode, varPop, varPush);
        if (StackCount < 0)
            throw new CodeGenerationException($"Negative stack count at {opCode} offset {generator.ILOffset}");
    }
}
