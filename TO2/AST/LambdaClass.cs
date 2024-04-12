using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST;

internal readonly struct LambdaClass {
    internal readonly List<(string sourceName, ClonedFieldVariable target)> clonedVariables;
    internal readonly ConstructorInfo constructor;
    internal readonly MethodInfo lambdaImpl;

    internal LambdaClass(List<(string sourceName, ClonedFieldVariable target)> clonedVariables,
        ConstructorInfo constructor, MethodInfo lambdaImpl) {
        this.clonedVariables = clonedVariables;
        this.constructor = constructor;
        this.lambdaImpl = lambdaImpl;
    }

    internal static LambdaClass CreateLambdaClass(IBlockContext parent, FunctionType lambdaType,
        List<FunctionParameter> parameters, Expression expression, Position start, Position end) {
        var lambdaModuleContext =
            parent.ModuleContext.DefineSubContext($"Lambda{start.position}", typeof(object));

        var lambdaContext = new SyncBlockContext(lambdaModuleContext, false, "LambdaImpl",
            lambdaType.returnType, FixedParameters(parameters, lambdaType));
        var clonedVariables =
            new SortedDictionary<string, (string sourceName, ClonedFieldVariable target)>();

        lambdaContext.ExternVariables = name => {
            if (clonedVariables.ContainsKey(name)) return clonedVariables[name].target;
            var externalVariable = parent.FindVariable(name);
            if (externalVariable == null) return null;
            if (!externalVariable.IsConst) {
                lambdaContext.AddError(new StructuralError(StructuralError.ErrorType.NoSuchVariable,
                    $"Outer variable {name} is not const. Only read-only variables can be referenced in a lambda expression",
                    start, end));
                return null;
            }

            var field = lambdaModuleContext.typeBuilder.DefineField(name,
                externalVariable.Type.GeneratedType(parent.ModuleContext),
                FieldAttributes.InitOnly | FieldAttributes.Private);
            var clonedVariable = new ClonedFieldVariable(externalVariable.Type, field);
            clonedVariables.Add(name, (externalVariable.Name, clonedVariable));
            return clonedVariable;
        };

        expression.EmitCode(lambdaContext, false);
        lambdaContext.IL.EmitReturn(lambdaContext.MethodBuilder!.ReturnType);

        foreach (var error in lambdaContext.AllErrors) parent.AddError(error);

        var lambdaFields = clonedVariables.Values.Select(c => c.target.valueField).ToList();
        var constructorBuilder = lambdaModuleContext.typeBuilder.DefineConstructor(
            MethodAttributes.Public, CallingConventions.Standard,
            lambdaFields.Select(f => f.FieldType).ToArray());
        IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());

        var argIndex = 1;
        foreach (var field in lambdaFields) {
            constructorEmitter.Emit(OpCodes.Ldarg_0);
            MethodParameter.EmitLoadArg(constructorEmitter, argIndex++);
            constructorEmitter.Emit(OpCodes.Stfld, field);
        }

        constructorEmitter.EmitReturn(typeof(void));

        lambdaType.GeneratedType(parent.ModuleContext);

        return new LambdaClass([.. clonedVariables.Values], constructorBuilder, lambdaContext.MethodBuilder);
    }
    private static List<FunctionParameter> FixedParameters(List<FunctionParameter> parameters, FunctionType lambdaType) =>
        parameters.Zip(lambdaType.parameterTypes, (p, f) => new FunctionParameter(p.name, p.type ?? f, null))
            .ToList();

}
