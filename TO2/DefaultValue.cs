using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2;

public interface IDefaultValue {
    void EmitCode(IBlockContext context);
}

public static class DefaultValue {
    public static IDefaultValue? ForParameter(IBlockContext context, FunctionParameter parameter) {
        if (parameter.defaultValue == null) return null;
        switch (parameter.defaultValue) {
        case LiteralBool b when parameter.type == BuiltinType.Bool: return new BoolDefaultValue(b.value);
        case LiteralInt i when parameter.type == BuiltinType.Int: return new IntDefaultValue(i.value);
        case LiteralInt i when parameter.type == BuiltinType.Float: return new FloatDefaultValue(i.value);
        case LiteralFloat f when parameter.type == BuiltinType.Float: return new FloatDefaultValue(f.value);
        case LiteralString s when parameter.type == BuiltinType.String: return new StringDefaultValue(s.value);
        default:
            IBlockContext defaultContext = new SyncBlockContext(context.ModuleContext, FunctionModifier.Public,
                false, $"default_{context.MethodBuilder!.Name}_{parameter.name}", parameter.type!,
                new List<FunctionParameter>());
            var resultType = parameter.defaultValue.ResultType(defaultContext);

            if (!parameter.type!.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Default value of parameter {parameter.name} has to be of type {parameter.type}, found {resultType}",
                    parameter.Start,
                    parameter.End
                ));
                return null;
            }

            parameter.defaultValue.EmitCode(defaultContext, false);
            parameter.type.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);
            defaultContext.IL.EmitReturn(parameter.type.GeneratedType(context.ModuleContext));

            foreach (var error in defaultContext.AllErrors) context.AddError(error);

            return new DefaultValueFactoryFunction(defaultContext.MethodBuilder!);
        }
    }
}

public class BoolDefaultValue : IDefaultValue {
    public BoolDefaultValue(bool value) {
        this.Value = value;
    }

    public bool Value { get; }

    public void EmitCode(IBlockContext context) {
        context.IL.Emit(Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }
}

public class IntDefaultValue : IDefaultValue {
    public IntDefaultValue(long value) {
        this.Value = value;
    }

    public long Value { get; }

    public void EmitCode(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldc_I8, Value);
    }
}

public class FloatDefaultValue : IDefaultValue {
    public FloatDefaultValue(double value) {
        this.Value = value;
    }

    public double Value { get; }

    public void EmitCode(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldc_R8, Value);
    }
}

public class EnumDefaultValue : IDefaultValue {
    private readonly int intValue;

    public EnumDefaultValue(Enum value) {
        intValue = (int)Convert.ChangeType(value, typeof(int));
    }

    public void EmitCode(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldc_I4, intValue);
    }
}

public class StringDefaultValue : IDefaultValue {
    public StringDefaultValue(string value) {
        this.Value = value;
    }

    public string Value { get; }

    public void EmitCode(IBlockContext context) {
        context.IL.Emit(OpCodes.Ldstr, Value);
    }
}

public class DefaultValueFactoryFunction : IDefaultValue {
    private readonly MethodInfo method;

    public DefaultValueFactoryFunction(MethodInfo method) {
        this.method = method;
    }

    public void EmitCode(IBlockContext context) {
        context.IL.EmitCall(OpCodes.Call, method, 0);
    }
}
