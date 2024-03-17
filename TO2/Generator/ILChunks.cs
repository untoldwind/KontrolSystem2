using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Generator;

public class ILChunks {
    public static void GenerateCheckTimeout(IBlockContext context) {
        context.IL.EmitCall(OpCodes.Call, typeof(ContextHolder).GetMethod("CheckTimeout")!, 0);
    }

    public static void GenerateFunctionEnter(IBlockContext context, string name, List<FunctionParameter> parameters, string sourceName, int line) {
        context.IL.Emit(OpCodes.Ldstr, context.ModuleContext.moduleName + "::" + name);
        context.IL.Emit(OpCodes.Ldc_I4, parameters.Count);
        context.IL.Emit(OpCodes.Newarr, typeof(object));
        for (var i = 0; i < parameters.Count; i++) {
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4, i);
            MethodParameter.EmitLoadArg(context.IL, i);
            var type = parameters[i].type!.GeneratedType(context.ModuleContext);
            if (type.IsValueType) context.IL.Emit(OpCodes.Box, type);
            context.IL.Emit(OpCodes.Stelem, typeof(object));
        }
        context.IL.Emit(OpCodes.Ldstr, sourceName);
        context.IL.Emit(OpCodes.Ldc_I4, line);

        context.IL.EmitCall(OpCodes.Call, typeof(ContextHolder).GetMethod("FunctionEnter")!, 4);
    }

    public static void GenerateFunctionLeave(IBlockContext context) {
        context.IL.EmitCall(OpCodes.Call, typeof(ContextHolder).GetMethod("FunctionLeave")!, 0);
    }

    public static void GenerateCallSite(IBlockContext context, string name, string sourceName, int line) {
        context.IL.Emit(OpCodes.Ldstr, name);
        context.IL.Emit(OpCodes.Ldstr, sourceName);
        context.IL.Emit(OpCodes.Ldc_I4, line);

        context.IL.EmitCall(OpCodes.Call, typeof(ContextHolder).GetMethod("SetCallSite")!, 3);
    }
}
