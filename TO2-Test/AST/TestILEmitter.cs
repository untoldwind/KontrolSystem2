using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using System.Linq;
using System.Globalization;

namespace KontrolSystem.TO2.Test.AST {
    public interface IILCommand {
    }

    public struct ILCommand : IILCommand {
        public OpCode opCode;
        public String args;

        public override string ToString() => String.IsNullOrEmpty(args) ? opCode.ToString() : $"{opCode} {args}";
    }

    public struct DeclareLocal : IILCommand {
        public int localIdx;
        public Type type;

        public override string ToString() => $"<declare local> {localIdx} = {type}";
    }

    public struct ScopeBegin : IILCommand {
    }

    public struct ScopeEnd : IILCommand {
    }

    public class TestILEmitter : IILEmitter {
        private int ilSize;
        private int stackCount;
        private int lastLocalIndex;
        public List<IILCommand> commands = new List<IILCommand>();

        public void Emit(OpCode opCode) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode });
        }

        public void Emit(OpCode opCode, byte arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X2") });
        }

        public void Emit(OpCode opCode, short arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X4") });
        }

        public void Emit(OpCode opCode, int arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X8") });
        }

        public void Emit(OpCode opCode, long arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("X16") });
        }

        public void Emit(OpCode opCode, double arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg.ToString("E", CultureInfo.InvariantCulture) });
        }

        public void Emit(OpCode opCode, string arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = arg });
        }

        public void Emit(OpCode opCode, ILocalRef localBuilder) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = $"<local>{localBuilder.LocalIndex}" });
        }

        public void Emit(OpCode opCode, LabelRef labelRef) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = $"<label>{labelRef.label.GetHashCode()}" });
        }

        public void Emit(OpCode opCode, IEnumerable<LabelRef> labels) {
            var labelRefs = labels.ToList();
            ilSize += InstructionSize.Get(opCode, labelRefs.Count());
            AdjustStack(opCode);
            commands.Add(new ILCommand {
                opCode = opCode,
                args = String.Join(", ", labelRefs.Select(label => $"<label>{label.label.GetHashCode()}"))
            });
        }

        public void Emit(OpCode opCode, FieldInfo field) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = $"<field>{field.Name}" });
        }

        public void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount, resultCount);
            commands.Add(new ILCommand { opCode = opCode, args = $"<type>{type}" });
        }

        public void EmitPtr(OpCode opCode, MethodInfo methodInfo) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
            commands.Add(new ILCommand { opCode = opCode, args = $"<method>{methodInfo}" });
        }


        public void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null,
            int? resultCount = null) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount ?? constructor.GetParameters().Length, resultCount);
        }

        public void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount, methodInfo.ReturnType != typeof(void) ? 1 : 0);
            commands.Add(new ILCommand { opCode = opCode, args = $"<method>{methodInfo.Name}" });
        }

        public ILocalRef DeclareLocal(Type type) {
            lastLocalIndex++;
            commands.Add(new DeclareLocal { localIdx = lastLocalIndex, type = type });
            return new CountingLocalRef(lastLocalIndex, type);
        }

        public ITempLocalRef TempLocal(Type type) {
            lastLocalIndex++;
            commands.Add(new DeclareLocal { localIdx = lastLocalIndex, type = type });
            return new CountingLocalRef(lastLocalIndex, type);
        }

        public LabelRef DefineLabel(bool isShort) => new LabelRef(new Label(), isShort);

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
                for (int i = stackCount; i > 0; i--) Emit(OpCodes.Pop);
                ilSize += InstructionSize.Get(OpCodes.Ret);
                AdjustStack(OpCodes.Ret, 0);
            } else {
                for (int i = stackCount - 1; i > 0; i--) Emit(OpCodes.Pop);
                if (stackCount == 0) Emit(OpCodes.Ldc_I4_0);
                ilSize += InstructionSize.Get(OpCodes.Ret);
                AdjustStack(OpCodes.Ret, 1);
            }

            commands.Add(new ILCommand { opCode = OpCodes.Ret, args = $"<type>{returnType}" });
        }

        public int LastLocalIndex => lastLocalIndex;

        public int ILSize => ilSize;

        public int StackCount => stackCount;

        public void AdjustStack(int diff) => stackCount += diff;

        private void AdjustStack(OpCode opCode, int? varPop = null, int? varPush = null) {
            stackCount += InstructionSize.StackDiff(opCode, varPop, varPush);
            if (stackCount < 0) throw new CodeGenerationException($"Negative stack count at {opCode} offset {ilSize}");
        }
    }
}
