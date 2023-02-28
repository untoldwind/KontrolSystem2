using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public delegate IBlockVariable VariableResolver(string name);

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

    internal class MethodParameter : IBlockVariable {
        private readonly int index;
        public string Name { get; }
        public RealizedType Type { get; }
        public bool IsConst { get; }

        public MethodParameter(string name, RealizedType type, int index, bool isConst = true) {
            Name = name;
            Type = type;
            this.index = index;
            IsConst = isConst;
        }

        public void EmitLoad(IBlockContext context) => EmitLoadArg(context.IL, index);

        public void EmitLoadPtr(IBlockContext context) {
            if (index < 256) context.IL.Emit(OpCodes.Ldarga_S, (byte)index);
            else context.IL.Emit(OpCodes.Ldarga, (short)index);
        }

        public void EmitStore(IBlockContext context) {
            if (index < 256) {
                context.IL.Emit(OpCodes.Starg_S, (byte)index);
            } else {
                context.IL.Emit(OpCodes.Starg, (short)index);
            }
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

    internal class DeclaredVariable : IBlockVariable {
        private readonly ILocalRef localRef;
        public string Name { get; }
        public RealizedType Type { get; }
        public bool IsConst { get; }

        public DeclaredVariable(string name, bool isConst, RealizedType type, ILocalRef localRef) {
            Name = name;
            IsConst = isConst;
            Type = type;
            this.localRef = localRef;
        }

        public void EmitLoad(IBlockContext context) => localRef.EmitLoad(context);

        public void EmitLoadPtr(IBlockContext context) => localRef.EmitLoadPtr(context);

        public void EmitStore(IBlockContext context) => localRef.EmitStore(context);
    }

    public class TempVariable : ITempBlockVariable {
        private readonly ITempLocalRef localRef;
        public RealizedType Type { get; }

        public TempVariable(RealizedType type, ITempLocalRef localRef) {
            Type = type;
            this.localRef = localRef;
        }

        public string Name => "***temp***";

        public bool IsConst => false;

        public void EmitLoad(IBlockContext context) => localRef.EmitLoad(context);

        public void EmitLoadPtr(IBlockContext context) => localRef.EmitLoadPtr(context);

        public void EmitStore(IBlockContext context) => localRef.EmitStore(context);
        public void Dispose() => localRef.Dispose();
    }

    public class ClonedFieldVariable : IBlockVariable {
        public readonly FieldInfo valueField;
        public RealizedType Type { get; }

        public ClonedFieldVariable(RealizedType type, FieldInfo valueField) {
            Type = type;
            this.valueField = valueField;
        }

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
}
