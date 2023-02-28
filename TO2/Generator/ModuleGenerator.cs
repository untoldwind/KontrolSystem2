using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public static class ModuleGenerator {
        public static DeclaredKontrolModule DeclareModule(Context context, TO2Module module) {
            ModuleContext moduleContext = context.CreateModuleContext(module.name);

            List<StructuralError> errors = module.TryDeclareTypes(moduleContext);

            if (errors.Any()) throw new CompilationErrorException(errors);

            return new DeclaredKontrolModule(module.name, module.description, moduleContext, module,
                moduleContext.exportedTypes);
        }

        public static void ImportTypes(DeclaredKontrolModule declaredModule) {
            ModuleContext moduleContext = declaredModule.moduleContext;

            List<StructuralError> errors = declaredModule.to2Module.TryImportTypes(moduleContext);

            if (errors.Any()) throw new CompilationErrorException(errors);
        }

        public static void DeclareFunctions(DeclaredKontrolModule declaredModule) {
            ModuleContext moduleContext = declaredModule.moduleContext;

            foreach (ConstDeclaration constant in declaredModule.to2Module.constants) {
                FieldInfo runtimeField = moduleContext.typeBuilder.DefineField($"const_{constant.name}",
                    constant.type.GeneratedType(moduleContext),
                    constant.isPublic
                        ? FieldAttributes.Public | FieldAttributes.Static
                        : FieldAttributes.Private | FieldAttributes.Static);
                DeclaredKontrolConstant declaredConstant =
                    new DeclaredKontrolConstant(declaredModule, constant, runtimeField);

                if (moduleContext.mappedConstants.ContainsKey(declaredConstant.Name))
                    throw new CompilationErrorException(new List<StructuralError> {
                        new StructuralError(
                            StructuralError.ErrorType.DuplicateConstantName,
                            $"Module {declaredModule.Name} already defines a constant {declaredConstant.Name}",
                            constant.Start,
                            constant.End
                        )
                    });

                moduleContext.mappedConstants.Add(declaredConstant.Name, declaredConstant);
                declaredModule.declaredConstants.Add(declaredConstant.Name, declaredConstant);
            }

            foreach (FunctionDeclaration function in declaredModule.to2Module.functions) {
                IBlockContext methodContext = moduleContext.CreateMethodContext(function.modifier, function.isAsync,
                    function.name, function.declaredReturn, function.parameters);
                DeclaredKontrolFunction declaredFunction =
                    new DeclaredKontrolFunction(declaredModule, methodContext, function);

                if (moduleContext.mappedFunctions.ContainsKey(declaredFunction.Name))
                    throw new CompilationErrorException(new List<StructuralError> {
                        new StructuralError(
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

            foreach (StructDeclaration to2Struct in declaredModule.to2Module.structs) {
                IBlockContext methodContext = moduleContext.CreateMethodContext(
                    to2Struct.exported ? FunctionModifier.Public : FunctionModifier.Private, false,
                    to2Struct.name, to2Struct.typeDelegate, to2Struct.constructorParameters);
                DeclaredKontrolStructConstructor declaredConstructor =
                    new DeclaredKontrolStructConstructor(declaredModule, methodContext, to2Struct);

                moduleContext.mappedFunctions.Add(declaredConstructor.Name, declaredConstructor);
                declaredModule.declaredStructConstructors.Add(declaredConstructor);
                if (to2Struct.exported)
                    declaredModule.publicFunctions.Add(declaredConstructor.Name, declaredConstructor);
            }
        }

        public static void ImportFunctions(DeclaredKontrolModule declaredModule) {
            ModuleContext moduleContext = declaredModule.moduleContext;

            List<StructuralError> errors = declaredModule.to2Module.TryImportConstants(moduleContext);

            if (errors.Any()) throw new CompilationErrorException(errors);

            errors = declaredModule.to2Module.TryImportFunctions(moduleContext);

            if (errors.Any()) throw new CompilationErrorException(errors);
        }

        public static void VerifyFunctions(DeclaredKontrolModule declaredModule) {
            ModuleContext moduleContext = declaredModule.moduleContext;

            List<StructuralError> errors = declaredModule.to2Module.TryVerifyFunctions(moduleContext);

            if (errors.Any()) throw new CompilationErrorException(errors);
        }

        public static void CompileStructs(DeclaredKontrolModule declaredModule) {
            List<StructuralError> errors = new List<StructuralError>();

            foreach (DeclaredKontrolStructConstructor structConstructor in declaredModule.declaredStructConstructors) {
                IBlockContext methodContext = structConstructor.methodContext;

                structConstructor.to2Struct.EmitConstructor(methodContext);
                errors.AddRange(methodContext.AllErrors);
            }
            if (errors.Any()) throw new CompilationErrorException(errors);
        }

        public static CompiledKontrolModule CompileModule(DeclaredKontrolModule declaredModule) {
            ModuleContext moduleContext = declaredModule.moduleContext;

            List<StructuralError> errors = new List<StructuralError>();
            List<CompiledKontrolConstant> compiledConstants = new List<CompiledKontrolConstant>();
            SyncBlockContext constructorContext = new SyncBlockContext(moduleContext);

            foreach (DeclaredKontrolConstant constant in declaredModule.declaredConstants.Values) {
                TO2Type expressionType = constant.to2Constant.expression.ResultType(constructorContext);

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
                if (constructorContext.HasErrors) {
                    errors.AddRange(constructorContext.AllErrors);
                } else {
                    constructorContext.IL.Emit(OpCodes.Stsfld, constant.runtimeField);
                }

                if (constant.IsPublic)
                    compiledConstants.Add(new CompiledKontrolConstant(constant.Name, constant.Description,
                        constant.Type, constant.runtimeField));
            }

            foreach (DeclaredKontrolFunction function in declaredModule.declaredFunctions) {
                IBlockContext methodContext = function.methodContext;

                function.to2Function.EmitCode(methodContext);
                errors.AddRange(methodContext.AllErrors);
            }

            if (errors.Any()) throw new CompilationErrorException(errors);

            Type runtimeType = moduleContext.CreateType();

            List<CompiledKontrolFunction> compiledFunctions = new List<CompiledKontrolFunction>();
            List<CompiledKontrolFunction> testFunctions = new List<CompiledKontrolFunction>();
            foreach (DeclaredKontrolFunction function in declaredModule.declaredFunctions) {
                if (function.to2Function.modifier == FunctionModifier.Private) continue;
                MethodBuilder methodBuilder = function.methodContext.MethodBuilder;
                MethodInfo methodInfo = runtimeType.GetMethod(methodBuilder.Name,
                    methodBuilder.GetParameters().Select(p => p.ParameterType).ToArray());
                CompiledKontrolFunction compiledFunction = new CompiledKontrolFunction(function.Name,
                    function.Description, function.IsAsync, function.Parameters, function.ReturnType, methodInfo);

                if (function.to2Function.modifier == FunctionModifier.Test) testFunctions.Add(compiledFunction);
                else compiledFunctions.Add(compiledFunction);
            }

            return new CompiledKontrolModule(declaredModule.Name,
                declaredModule.Description,
                moduleContext.exportedTypes.Select(t => (t.alias, t.type.UnderlyingType(moduleContext))),
                compiledConstants,
                compiledFunctions,
                testFunctions);
        }
    }
}
