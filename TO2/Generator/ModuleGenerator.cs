using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator;

public static class ModuleGenerator {
    public static DeclaredKontrolModule DeclareModule(Context context, TO2Module module, string sourceFile) {
        var moduleContext = context.CreateModuleContext(module.name);

        var errors = module.TryDeclareTypes(moduleContext);

        if (errors.Any()) throw new CompilationErrorException(errors);

        return new DeclaredKontrolModule(module.name, module.description, false, sourceFile, moduleContext, module,
            moduleContext.exportedTypes!);
    }

    public static void ImportTypes(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        var errors = declaredModule.to2Module.TryImportTypes(moduleContext);

        if (errors.Any()) throw new CompilationErrorException(errors);
    }

    public static void DeclareConstants(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        foreach (var constant in declaredModule.to2Module.constants) {
            FieldInfo runtimeField = moduleContext.typeBuilder.DefineField($"const_{constant.name}",
                constant.type.GeneratedType(moduleContext),
                constant.isPublic
                    ? FieldAttributes.Public | FieldAttributes.Static
                    : FieldAttributes.Private | FieldAttributes.Static);
            var declaredConstant =
                new DeclaredKontrolConstant(declaredModule, constant, runtimeField);

            if (moduleContext.mappedConstants.ContainsKey(declaredConstant.Name))
                throw new CompilationErrorException(new List<StructuralError> {
                    new(
                        StructuralError.ErrorType.DuplicateConstantName,
                        $"Module {declaredModule.Name} already defines a constant {declaredConstant.Name}",
                        constant.Start,
                        constant.End
                    )
                });

            moduleContext.mappedConstants.Add(declaredConstant.Name, declaredConstant);
            declaredModule.declaredConstants.Add(declaredConstant.Name, declaredConstant);
        }
    }

    public static void ImportConstants(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        var errors = declaredModule.to2Module.TryImportConstants(moduleContext);

        if (errors.Any()) throw new CompilationErrorException(errors);
    }

    public static void DeclareFunctions(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        foreach (var function in declaredModule.to2Module.functions) {
            var methodContext = moduleContext.CreateMethodContext(function.modifier, function.isAsync,
                function.name, function.declaredReturn, function.parameters);
            var declaredFunction =
                new DeclaredKontrolFunction(declaredModule, methodContext, function);

            if (moduleContext.mappedFunctions.ContainsKey(declaredFunction.Name))
                throw new CompilationErrorException(new List<StructuralError> {
                    new(
                        StructuralError.ErrorType.DuplicateFunctionName,
                        $"Module {declaredModule.Name} already defines a function {declaredFunction.Name}",
                        function.Start,
                        function.End
                    )
                });

            moduleContext.mappedFunctions.Add(declaredFunction.Name, declaredFunction);
            declaredModule.declaredFunctions.Add(declaredFunction);
            if (function.modifier == FunctionModifier.Public)
                declaredModule.publicFunctions.Add(declaredFunction.Name, declaredFunction);
        }

        foreach (var to2Struct in declaredModule.to2Module.structs) {
            var methodContext = moduleContext.CreateMethodContext(
                to2Struct.exported ? FunctionModifier.Public : FunctionModifier.Private, false,
                to2Struct.name, to2Struct.typeDelegate!, to2Struct.constructorParameters);
            var declaredConstructor =
                new DeclaredKontrolStructConstructor(declaredModule, methodContext, to2Struct);

            moduleContext.mappedFunctions.Add(declaredConstructor.Name, declaredConstructor);
            declaredModule.declaredStructConstructors.Add(declaredConstructor);
            if (to2Struct.exported)
                declaredModule.publicFunctions.Add(declaredConstructor.Name, declaredConstructor);
        }
    }

    public static void ImportFunctions(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        var errors = declaredModule.to2Module.TryImportFunctions(moduleContext);

        if (errors.Any()) throw new CompilationErrorException(errors);
    }

    public static void VerifyFunctions(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        var errors = declaredModule.to2Module.TryVerifyFunctions(moduleContext);

        if (errors.Any()) throw new CompilationErrorException(errors);
    }

    public static void CompileStructs(DeclaredKontrolModule declaredModule) {
        var errors = new List<StructuralError>();

        foreach (var structConstructor in declaredModule.declaredStructConstructors) {
            var methodContext = structConstructor.methodContext;

            structConstructor.to2Struct.EmitConstructor(methodContext);
            errors.AddRange(methodContext.AllErrors);
        }

        if (errors.Any()) throw new CompilationErrorException(errors);
    }

    public static CompiledKontrolModule CompileModule(DeclaredKontrolModule declaredModule) {
        var moduleContext = declaredModule.moduleContext;

        var errors = new List<StructuralError>();
        var compiledConstants = new List<CompiledKontrolConstant>();
        var constructorContext = new SyncBlockContext(moduleContext);

        foreach (var constant in declaredModule.declaredConstants.Values) {
            var expressionType = constant.to2Constant.expression.ResultType(constructorContext);

            if (!constant.Type.IsAssignableFrom(constructorContext.ModuleContext, expressionType)) {
                errors.Add(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Constant {constant.Name} can not be initialized with type {expressionType}",
                    constant.to2Constant.Start,
                    constant.to2Constant.End
                ));
                continue;
            }

            constant.to2Constant.expression.EmitCode(constructorContext, false);
            constant.Type.AssignFrom(constructorContext.ModuleContext, expressionType)
                .EmitConvert(constructorContext);
            if (constructorContext.HasErrors)
                errors.AddRange(constructorContext.AllErrors);
            else
                constructorContext.IL.Emit(OpCodes.Stsfld, constant.runtimeField);

            if (constant.IsPublic)
                compiledConstants.Add(new CompiledKontrolConstant(constant.Name, constant.Description,
                    constant.Type, constant.runtimeField));
        }

        foreach (var function in declaredModule.declaredFunctions) {
            var methodContext = function.methodContext;

            try {
                function.to2Function.EmitCode(methodContext);
            } catch (CompilationErrorException e) {
                errors.AddRange(e.errors);
            }

            errors.AddRange(methodContext.AllErrors);
        }

        if (errors.Any()) throw new CompilationErrorException(errors);

        var runtimeType = moduleContext.CreateType();

        var compiledFunctions = new List<CompiledKontrolFunction>();
        var testFunctions = new List<CompiledKontrolFunction>();
        foreach (var function in declaredModule.declaredFunctions) {
            if (function.to2Function.modifier == FunctionModifier.Private) continue;
            var methodBuilder = function.methodContext.MethodBuilder;
            var methodInfo = runtimeType.GetMethod(methodBuilder!.Name,
                methodBuilder.GetParameters().Select(p => p.ParameterType).ToArray());
            var compiledFunction = new CompiledKontrolFunction(function.Name,
                function.Description, function.IsAsync, function.Parameters, function.ReturnType, methodInfo);

            if (function.to2Function.modifier == FunctionModifier.Test) testFunctions.Add(compiledFunction);
            else compiledFunctions.Add(compiledFunction);
        }

        foreach (var constructor in declaredModule.declaredStructConstructors) {
            if (!constructor.to2Struct.exported) continue;
            var compiledFunction = new CompiledKontrolFunction(constructor.Name,
                constructor.Description, constructor.IsAsync, constructor.Parameters, constructor.ReturnType,
                constructor.RuntimeMethod);
            compiledFunctions.Add(compiledFunction);
        }

        return new CompiledKontrolModule(declaredModule.Name,
            declaredModule.Description,
            declaredModule.IsBuiltin,
            declaredModule.SourceFile,
            moduleContext.exportedTypes!.Select(t => (t.alias, t.type.UnderlyingType(moduleContext))),
            compiledConstants,
            compiledFunctions,
            testFunctions);
    }
}
