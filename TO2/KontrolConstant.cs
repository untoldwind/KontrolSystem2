using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2;

public interface IKontrolConstant {
    string Name { get; }

    string? Description { get; }

    TO2Type Type { get; }

    void EmitLoad(IBlockContext context);

    IREPLValue REPLValue();
}

public class CompiledKontrolConstant(string name, string? description, TO2Type type, FieldInfo runtimeField)
    : IKontrolConstant {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public TO2Type Type { get; } = type;

    public void EmitLoad(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldsfld, runtimeField);
    }

    public IREPLValue REPLValue() {
        return Type.REPLCast(runtimeField.GetValue(null));
    }
}

public class EnumKontrolConstant(string name, BoundEnumConstType type, string description) : IKontrolConstant {
    public string Name { get; } = name;

    public TO2Type Type => type;

    public string Description { get; } = description;

    public void EmitLoad(IBlockContext context) {
    }

    public IREPLValue REPLValue() {
        return REPLUnit.INSTANCE;
    }
}

public class DeclaredKontrolConstant(DeclaredKontrolModule module, ConstDeclaration to2Constant, FieldInfo runtimeField)
    : IKontrolConstant {
    public readonly FieldInfo runtimeField = runtimeField;
    public readonly ConstDeclaration to2Constant = to2Constant;

    public bool IsPublic => to2Constant.isPublic;

    public string Name => to2Constant.name;

    public string Description => to2Constant.description;

    public TO2Type Type => to2Constant.type.UnderlyingType(module.moduleContext);

    public void EmitLoad(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldsfld, runtimeField);
    }

    public IREPLValue REPLValue() {
        return Type.REPLCast(runtimeField.GetValue(null));
    }
}
