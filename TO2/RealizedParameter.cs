using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2;

public readonly struct RealizedParameter {
    public readonly string name;
    public readonly RealizedType type;
    public readonly string? description;
    public readonly IDefaultValue? defaultValue;

    public RealizedParameter(string name, RealizedType type, string? description, IDefaultValue? defaultValue = null) {
        this.name = name;
        this.type = type;
        this.description = description;
        this.defaultValue = defaultValue;
    }

    public RealizedParameter(IBlockContext context, FunctionParameter parameter) {
        name = parameter.name;
        type = parameter.type!.UnderlyingType(context.ModuleContext);
        description = null;
        defaultValue = DefaultValue.ForParameter(context, parameter);
    }

    public bool HasDefault => defaultValue != null;

    public RealizedParameter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return new RealizedParameter(name, type.FillGenerics(context, typeArguments), description, defaultValue);
    }
}
