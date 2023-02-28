using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
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
    }

    public class Lambda : Expression, IVariableContainer {
        private readonly List<FunctionParameter> parameters;
        private readonly Expression expression;

        private IVariableContainer parentContainer;

        private TypeHint typeHint;
        private LambdaClass? lambdaClass;

        private FunctionType resolvedType;

        public Lambda(List<FunctionParameter> parameters, Expression expression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.parameters = parameters;
            this.expression = expression;
            this.expression.VariableContainer = this;
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) {
            int idx = parameters.FindIndex(p => p.name == name);

            if (idx < 0 || idx >= parameters.Count) return null;

            TO2Type parameterType = parameters[idx].type;
            if (parameterType != null) return parameterType;
            if (resolvedType == null || idx >= resolvedType.parameterTypes.Count) return null;

            return resolvedType.parameterTypes[idx];
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                expression.VariableContainer = this;
            }
        }

        public override TypeHint TypeHint {
            set => this.typeHint = value;
        }

        public override void Prepare(IBlockContext context) {
        }

        public override TO2Type ResultType(IBlockContext context) {
            if (resolvedType != null) return resolvedType;
            // Make an assumption ...
            if (parameters.All(p => p.type != null))
                resolvedType = new FunctionType(false, parameters.Select(p => p.type).ToList(), BuiltinType.Unit);
            else resolvedType = typeHint?.Invoke(context) as FunctionType;
            if (resolvedType != null) {
                // ... so that it is possible to determine the return type
                TO2Type returnType = expression.ResultType(context);
                // ... so that the assumption can be replaced with the (hopefully) real thing
                resolvedType = new FunctionType(false, resolvedType.parameterTypes, returnType);
            }

            return resolvedType ?? BuiltinType.Unit;
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            FunctionType lambdaType = ResultType(context) as FunctionType;

            if (lambdaType == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    "Unable to infer type of lambda. Please add some type hint",
                    Start,
                    End
                ));
                return;
            }

            if (lambdaType.parameterTypes.Count != parameters.Count)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Expected lambda to have {lambdaType.parameterTypes.Count} parameters, found {parameters.Count}",
                    Start,
                    End
                ));

            for (int i = 0; i < parameters.Count; i++) {
                if (parameters[i].type == null) continue;
                if (!lambdaType.parameterTypes[i].IsAssignableFrom(context.ModuleContext, parameters[i].type))
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Expected parameter {parameters[i].name} of lambda to have type {lambdaType.parameterTypes[i]}, found {parameters[i].type}",
                        Start,
                        End
                    ));
            }

            if (context.HasErrors) return;

            if (dropResult) return;

            lambdaClass ??= CreateLambdaClass(context, lambdaType);

            foreach ((string sourceName, _) in lambdaClass.Value.clonedVariables) {
                IBlockVariable source = context.FindVariable(sourceName);
                source.EmitLoad(context);
            }

            context.IL.EmitNew(OpCodes.Newobj, lambdaClass.Value.constructor, lambdaClass.Value.clonedVariables.Count);
            context.IL.EmitPtr(OpCodes.Ldftn, lambdaClass.Value.lambdaImpl);
            context.IL.EmitNew(OpCodes.Newobj,
                lambdaType.GeneratedType(context.ModuleContext)
                    .GetConstructor(new[] { typeof(object), typeof(IntPtr) }));
        }

        private LambdaClass CreateLambdaClass(IBlockContext parent, FunctionType lambdaType) {
            ModuleContext lambdaModuleContext =
                parent.ModuleContext.DefineSubContext($"Lambda{Start.position}", typeof(object));

            SyncBlockContext lambdaContext = new SyncBlockContext(lambdaModuleContext, false, "LambdaImpl",
                lambdaType.returnType, FixedParameters(lambdaType));
            SortedDictionary<string, (string sourceName, ClonedFieldVariable target)> clonedVariables =
                new SortedDictionary<string, (string sourceName, ClonedFieldVariable target)>();

            lambdaContext.ExternVariables = name => {
                if (clonedVariables.ContainsKey(name)) return clonedVariables[name].target;
                IBlockVariable externalVariable = parent.FindVariable(name);
                if (externalVariable == null) return null;
                if (!externalVariable.IsConst) {
                    lambdaContext.AddError(new StructuralError(StructuralError.ErrorType.NoSuchVariable,
                        $"Outer variable {name} is not const. Only read-only variables can be referenced in a lambda expression",
                        Start, End));
                    return null;
                }

                FieldBuilder field = lambdaModuleContext.typeBuilder.DefineField(name,
                    externalVariable.Type.GeneratedType(parent.ModuleContext),
                    FieldAttributes.InitOnly | FieldAttributes.Private);
                ClonedFieldVariable clonedVariable = new ClonedFieldVariable(externalVariable.Type, field);
                clonedVariables.Add(name, (externalVariable.Name, clonedVariable));
                return clonedVariable;
            };

            expression.EmitCode(lambdaContext, false);
            lambdaContext.IL.EmitReturn(lambdaContext.MethodBuilder.ReturnType);

            foreach (StructuralError error in lambdaContext.AllErrors) parent.AddError(error);

            List<FieldInfo> lambdaFields = clonedVariables.Values.Select(c => c.target.valueField).ToList();
            ConstructorBuilder constructorBuilder = lambdaModuleContext.typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard,
                lambdaFields.Select(f => f.FieldType).ToArray());
            IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());

            int argIndex = 1;
            foreach (FieldInfo field in lambdaFields) {
                constructorEmitter.Emit(OpCodes.Ldarg_0);
                MethodParameter.EmitLoadArg(constructorEmitter, argIndex++);
                constructorEmitter.Emit(OpCodes.Stfld, field);
            }

            constructorEmitter.EmitReturn(typeof(void));

            lambdaType.GeneratedType(parent.ModuleContext);

            return new LambdaClass(clonedVariables.Values.ToList(), constructorBuilder, lambdaContext.MethodBuilder);
        }

        private List<FunctionParameter> FixedParameters(FunctionType lambdaType) =>
            parameters.Zip(lambdaType.parameterTypes, (p, f) => new FunctionParameter(p.name, p.type ?? f))
                .ToList();
    }
}
