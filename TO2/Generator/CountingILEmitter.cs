using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator;

public class CountingLocalRef : ITempLocalRef {
    public CountingLocalRef(int localIndex, Type localType) {
        LocalIndex = localIndex;
        LocalType = localType;
    }

    public int LocalIndex { get; }
    public Type LocalType { get; }

    public void Dispose() {
    }
}

public class CountingILEmitter : IILEmitter {
    public CountingILEmitter(int lastLocalIndex) {
        ILSize = 0;
        StackCount = 0;
        this.LastLocalIndex = lastLocalIndex;
    }

    public void Emit(OpCode opCode) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, byte arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, short arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, int arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, long arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, double arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, string arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, ILocalRef localBuilder) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, LabelRef labelRef) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, IEnumerable<LabelRef> labels) {
        ILSize += InstructionSize.Get(opCode, labels.Count());
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, FieldInfo field) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }

    public void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount, resultCount);
    }

    public void EmitPtr(OpCode opCode, MethodInfo methodInfo) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
    }


    public void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null,
        int? resultCount = null) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount ?? constructor.GetParameters().Length, resultCount);
    }

    public void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount, methodInfo.ReturnType != typeof(void) ? 1 : 0);
    }

    public ILocalRef DeclareLocal(Type type) {
        LastLocalIndex++;
        return new CountingLocalRef(LastLocalIndex, type);
    }

    public ITempLocalRef TempLocal(Type type) {
        LastLocalIndex++;
        return new CountingLocalRef(LastLocalIndex, type);
    }

    public LabelRef DefineLabel(bool isShort) {
        return new LabelRef(new Label(), isShort);
    }

    public void MarkLabel(LabelRef label) {
    }

    public void BeginScope() {
    }

    public void EndScope() {
    }

    public void EmitReturn(Type returnType) {
        if (returnType == typeof(void)) {
            for (var i = StackCount; i > 0; i--) Emit(OpCodes.Pop);
            ILSize += InstructionSize.Get(OpCodes.Ret);
            AdjustStack(OpCodes.Ret, 0);
        } else {
            for (var i = StackCount - 1; i > 0; i--) Emit(OpCodes.Pop);
            if (StackCount == 0) Emit(OpCodes.Ldc_I4_0);
            ILSize += InstructionSize.Get(OpCodes.Ret);
            AdjustStack(OpCodes.Ret, 1);
        }
    }

    public int LastLocalIndex { get; private set; }

    public int ILSize { get; private set; }

    public int StackCount { get; private set; }

    public void AdjustStack(int diff) {
        StackCount += diff;
    }

    private void AdjustStack(OpCode opCode, int? varPop = null, int? varPush = null) {
        StackCount += InstructionSize.StackDiff(opCode, varPop, varPush);
        if (StackCount < 0) throw new CodeGenerationException($"Negative stack count at {opCode} offset {ILSize}");
    }
}
