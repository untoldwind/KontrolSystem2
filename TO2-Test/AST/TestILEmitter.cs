using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Test.AST;

public interface IILCommand;

public struct ILCommand : IILCommand {
    public OpCode opCode;
    public string args;

    public override string? ToString() {
        return string.IsNullOrEmpty(args) ? opCode.ToString() : $"{opCode} {args}";
    }
}

public struct DeclareLocal : IILCommand {
    public int localIdx;
    public Type type;

    public override string ToString() {
        return $"<declare local> {localIdx} = {type}";
    }
}

public struct ScopeBegin : IILCommand;

public struct ScopeEnd : IILCommand;

public class TestILEmitter : IILEmitter {
    public List<IILCommand> commands = new();

    public void Emit(OpCode opCode) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode });
    }

    public void Emit(OpCode opCode, byte arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X2") });
    }

    public void Emit(OpCode opCode, short arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X4") });
    }

    public void Emit(OpCode opCode, int arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X8") });
    }

    public void Emit(OpCode opCode, long arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X16") });
    }

    public void Emit(OpCode opCode, double arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("E", CultureInfo.InvariantCulture) });
    }

    public void Emit(OpCode opCode, string arg) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = arg });
    }

    public void Emit(OpCode opCode, ILocalRef localBuilder) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = $"<local>{localBuilder.LocalIndex}" });
    }

    public void Emit(OpCode opCode, LabelRef labelRef) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = $"<label>{labelRef.label.GetHashCode()}" });
    }

    public void Emit(OpCode opCode, IEnumerable<LabelRef> labels) {
        var labelRefs = labels.ToList();
        ILSize += InstructionSize.Get(opCode, labelRefs.Count());
        AdjustStack(opCode);
        commands.Add(new ILCommand {
            opCode = opCode,
            args = string.Join(", ", labelRefs.Select(label => $"<label>{label.label.GetHashCode()}"))
        });
    }

    public void Emit(OpCode opCode, FieldInfo field) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = $"<field>{field.Name}" });
    }

    public void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount, resultCount);
        commands.Add(new ILCommand { opCode = opCode, args = $"<type>{type}" });
    }

    public void EmitPtr(OpCode opCode, MethodInfo methodInfo) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode);
        commands.Add(new ILCommand { opCode = opCode, args = $"<method>{methodInfo}" });
    }


    public void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null,
        int? resultCount = null) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount ?? constructor.GetParameters().Length, resultCount);
    }

    public void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount) {
        ILSize += InstructionSize.Get(opCode);
        AdjustStack(opCode, argumentCount, methodInfo.ReturnType != typeof(void) ? 1 : 0);
        commands.Add(new ILCommand { opCode = opCode, args = $"<method>{methodInfo.Name}" });
    }

    public ILocalRef DeclareLocal(Type type) {
        LastLocalIndex++;
        commands.Add(new DeclareLocal { localIdx = LastLocalIndex, type = type });
        return new CountingLocalRef(LastLocalIndex, type);
    }

    public ITempLocalRef TempLocal(Type type) {
        LastLocalIndex++;
        commands.Add(new DeclareLocal { localIdx = LastLocalIndex, type = type });
        return new CountingLocalRef(LastLocalIndex, type);
    }

    public LabelRef DefineLabel(bool isShort) {
        return new LabelRef(new Label(), isShort);
    }

    public void MarkLabel(LabelRef label) {
    }

    public void BeginScope() {
        commands.Add(new ScopeBegin());
    }

    public void EndScope() {
        commands.Add(new ScopeEnd());
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

        commands.Add(new ILCommand { opCode = OpCodes.Ret, args = $"<type>{returnType}" });
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
