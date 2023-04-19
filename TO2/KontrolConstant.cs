using System;
using KontrolSystem.TO2.AST;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2 {
    public interface IKontrolConstant {
        string Name { get; }

        string Description { get; }

        TO2Type Type { get; }

        void EmitLoad(IBlockContext context);

        IREPLValue REPLValue();
    }

    public class CompiledKontrolConstant : IKontrolConstant {
        public string Name { get; }
        public string Description { get; }
        public TO2Type Type { get; }

        private readonly FieldInfo runtimeField;

        public CompiledKontrolConstant(string name, string description, TO2Type type, FieldInfo runtimeField) {
            Name = name;
            Description = description;
            Type = type;
            this.runtimeField = runtimeField;
        }

        public void EmitLoad(IBlockContext context) {
            context.IL.Emit(OpCodes.Ldsfld, runtimeField);
        }

        public IREPLValue REPLValue() {
            return Type.REPLCast(runtimeField.GetValue(null));
        }
    }

    public class EnumKontrolConstant : IKontrolConstant {
        public string Name { get; }

        private BoundEnumConstType enumType;

        public EnumKontrolConstant(string name, BoundEnumConstType type, string description) {
            Name = name;
            enumType = type;
            Description = description;
        }

        public TO2Type Type => enumType;

        public string Description { get; }

        public void EmitLoad(IBlockContext context) {
        }

        public IREPLValue REPLValue() => REPLUnit.INSTANCE;
    }

    public class DeclaredKontrolConstant : IKontrolConstant {
        public readonly ConstDeclaration to2Constant;
        public readonly FieldInfo runtimeField;

        public DeclaredKontrolConstant(ConstDeclaration to2Constant, FieldInfo runtimeField) {
            this.to2Constant = to2Constant;
            this.runtimeField = runtimeField;
        }

        public string Name => to2Constant.name;

        public string Description => to2Constant.description;

        public TO2Type Type => to2Constant.type;

        public bool IsPublic => to2Constant.isPublic;

        public void EmitLoad(IBlockContext context) {
            context.IL.Emit(OpCodes.Ldsfld, runtimeField);
        }

        public IREPLValue REPLValue() {
            return Type.REPLCast(runtimeField.GetValue(null));
        }
    }
}
