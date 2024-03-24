using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Binding;

public static class BoundDefaultValue {
    public static IDefaultValue? DefaultValueFor(ParameterInfo parameter) {
        if (!parameter.HasDefaultValue) return null;
        if (parameter.DefaultValue == null) return new NullDefaultValue(parameter.ParameterType);
        return parameter.DefaultValue switch {
            bool b => new BoolDefaultValue(b),
            long l => new IntDefaultValue(l),
            double d => new FloatDefaultValue(d),
            string s => new StringDefaultValue(s),
            Enum e => new EnumDefaultValue(e),
            _ => throw new ArgumentException($"Unable to handle default value with type {parameter.ParameterType}"),
        };
    }
}

public class NullDefaultValue(Type type) : IDefaultValue {
    private readonly Type type = type;

    public void EmitCode(IBlockContext context) {
        if (type.IsValueType) {
            using var temp = context.IL.TempLocal(type);
            temp.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Initobj, type, 1, 0);
            temp.EmitLoad(context);
        } else {
            context.IL.Emit(OpCodes.Ldnull);
        }
    }
}
