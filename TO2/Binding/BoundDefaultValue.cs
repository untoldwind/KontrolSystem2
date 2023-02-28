using System;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Binding {
    public static class BoundDefaultValue {
        public static IDefaultValue DefaultValueFor(ParameterInfo parameter) {
            if (!parameter.HasDefaultValue) return null;
            if (parameter.DefaultValue == null) return new NullDefaultValue(parameter.ParameterType);
            switch (parameter.DefaultValue) {
            case bool b: return new BoolDefaultValue(b);
            case long l: return new IntDefaultValue(l);
            case double d: return new FloatDefaultValue(d);
            default: throw new ArgumentException($"Unable to handle default value with type {parameter.ParameterType}");
            }
        }
    }

    public class NullDefaultValue : IDefaultValue {
        private readonly Type type;

        public NullDefaultValue(Type type) => this.type = type;

        public void EmitCode(IBlockContext context) {
            if (type.IsValueType) {
                using ITempLocalRef temp = context.IL.TempLocal(type);
                temp.EmitLoadPtr(context);
                context.IL.Emit(OpCodes.Initobj, type, 1, 0);
                temp.EmitLoad(context);
            } else {
                context.IL.Emit(OpCodes.Ldnull);
            }
        }
    }
}
