using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.TO2.Generator {
    public class CountingLocalRef : ITempLocalRef {
        public int LocalIndex { get; }
        public Type LocalType { get; }

        public CountingLocalRef(int localIndex, Type localType) {
            LocalIndex = localIndex;
            LocalType = localType;
        }

        public void Dispose() {
        }
    }

    public class CountingILEmitter : IILEmitter {
        private int ilSize;
        private int stackCount;
        private int lastLocalIndex;

        public CountingILEmitter(int lastLocalIndex) {
            ilSize = 0;
            stackCount = 0;
            this.lastLocalIndex = lastLocalIndex;
        }

        public void Emit(OpCode opCode) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, byte arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, short arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, int arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, long arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, double arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, string arg) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, ILocalRef localBuilder) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, LabelRef labelRef) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, IEnumerable<LabelRef> labels) {
            ilSize += InstructionSize.Get(opCode, labels.Count());
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, FieldInfo field) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }

        public void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount, resultCount);
        }

        public void EmitPtr(OpCode opCode, MethodInfo methodInfo) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode);
        }


        public void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null,
            int? resultCount = null) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount ?? constructor.GetParameters().Length, resultCount);
        }

        public void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount) {
            ilSize += InstructionSize.Get(opCode);
            AdjustStack(opCode, argumentCount, methodInfo.ReturnType != typeof(void) ? 1 : 0);
        }

        public ILocalRef DeclareLocal(Type type) {
            lastLocalIndex++;
            return new CountingLocalRef(lastLocalIndex, type);
        }

        public ITempLocalRef TempLocal(Type type) {
            lastLocalIndex++;
            return new CountingLocalRef(lastLocalIndex, type);
        }

        public LabelRef DefineLabel(bool isShort) => new LabelRef(new Label(), isShort);

        public void MarkLabel(LabelRef label) {
        }

        public void BeginScope() {
        }

        public void EndScope() {
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
