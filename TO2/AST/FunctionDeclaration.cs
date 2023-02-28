using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public enum FunctionModifier {
        Public,
        Private,
        Test,
    }

    public class FunctionParameter : Node {
        public readonly string name;
        public readonly TO2Type type;
        public readonly Expression defaultValue;

        public FunctionParameter(string name, TO2Type type, Expression defaultValue = null,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public override string ToString() => $"{name} : {type}";
    }

    public class FunctionDeclaration : Node, IModuleItem, IVariableContainer {
        public readonly FunctionModifier modifier;
        public readonly string name;
        public readonly string description;
        public readonly List<FunctionParameter> parameters;
        public readonly TO2Type declaredReturn;
        private readonly Expression expression;
        public readonly bool isAsync;
        private AsyncClass? asyncClass;

        public FunctionDeclaration(FunctionModifier modifier, bool isAsync, string name, string description,
            List<FunctionParameter> parameters, TO2Type declaredReturn, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.modifier = modifier;
            this.name = name;
            this.description = description;
            this.isAsync = isAsync;
            this.parameters = parameters;
            this.declaredReturn = declaredReturn;
            this.expression = expression;
            this.expression.VariableContainer = this;
            this.expression.TypeHint = context => this.declaredReturn.UnderlyingType(context.ModuleContext);
        }

        public IVariableContainer ParentContainer => null;

        public TO2Type FindVariableLocal(IBlockContext context, string variableName) =>
            parameters.Find(p => p.name == variableName)?.type;

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
            List<StructuralError> errors =
                parameters.Select(p => p.type).Concat(new[] { declaredReturn })
                    .Where(type => !type.IsValid(context)).Select(
                        type => new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Invalid type name '{type.Name}'",
                            Start,
                            End
                        )).ToList();

            return errors;
        }

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public void EmitCode(IBlockContext context) {
            TO2Type valueType = expression.ResultType(context);
            if (declaredReturn != BuiltinType.Unit &&
                !declaredReturn.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                    Start,
                    End
                ));
                return;
            }

            if (isAsync) EmitCodeAsync(context);
            else EmitCodeSync(context);
        }

        private void EmitCodeSync(IBlockContext context) {
            expression.EmitCode(context, declaredReturn == BuiltinType.Unit);
            if (!context.HasErrors && declaredReturn != BuiltinType.Unit)
                declaredReturn.AssignFrom(context.ModuleContext, expression.ResultType(context)).EmitConvert(context);
            else if (declaredReturn == BuiltinType.Unit) {
                context.IL.Emit(OpCodes.Ldnull);
            }

            context.IL.EmitReturn(context.MethodBuilder.ReturnType);
        }

        private void EmitCodeAsync(IBlockContext context) {
            asyncClass ??= AsyncClass.Create(context, name, declaredReturn, parameters, expression);

            for (int idx = 0; idx < parameters.Count; idx++)
                MethodParameter.EmitLoadArg(context.IL, idx);
            context.IL.EmitNew(OpCodes.Newobj, asyncClass.Value.constructor, parameters.Count);
            context.IL.EmitReturn(asyncClass.Value.type);
        }
    }
}
