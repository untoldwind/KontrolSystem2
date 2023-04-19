using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class MethodDeclaration : Node, IVariableContainer {
        public readonly string name;
        private readonly string description;
        private readonly bool isAsync;
        //        private readonly bool isConst;
        private readonly List<FunctionParameter> parameters;
        private readonly TO2Type declaredReturn;
        private readonly Expression expression;
        private SyncBlockContext syncBlockContext;
        private StructTypeAliasDelegate structType;
        private AsyncClass? asyncClass;

        public MethodDeclaration(bool isAsync, string name, string description,
            List<FunctionParameter> parameters,
            TO2Type declaredReturn, Expression expression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.name = name;
            this.description = description;
            this.isAsync = isAsync;
            this.parameters = parameters;
            this.declaredReturn = declaredReturn;
            if (expression is Block b) {
                this.expression = b.CollapseFinalReturn();
            } else {
                this.expression = expression;
            }
            this.expression.VariableContainer = this;
            this.expression.TypeHint = context => this.declaredReturn.UnderlyingType(context.ModuleContext);
        }

        public IVariableContainer ParentContainer => null;

        public TO2Type FindVariableLocal(IBlockContext context, string variableName) =>
            variableName == "self" ? structType : parameters.Find(p => p.name == variableName)?.type;

        public StructTypeAliasDelegate StructType {
            set => structType = value;
        }

        public IMethodInvokeFactory CreateInvokeFactory() {
            syncBlockContext ??= new SyncBlockContext(structType, isAsync, structType.Name.ToUpper() + "_" + name, declaredReturn, parameters);

            return new BoundMethodInvokeFactory(
                description,
                true,
                () => declaredReturn.UnderlyingType(structType.structContext),
                () => parameters.Select(p => new RealizedParameter(syncBlockContext, p)).ToList(),
                isAsync,
                structType.structContext.typeBuilder,
                syncBlockContext.MethodBuilder
            );
        }

        public IEnumerable<StructuralError> EmitCode() {
            TO2Type valueType = expression.ResultType(syncBlockContext);
            if (declaredReturn != BuiltinType.Unit &&
                !declaredReturn.IsAssignableFrom(syncBlockContext.ModuleContext, valueType)) {
                syncBlockContext.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                    Start,
                    End
                ));
                return syncBlockContext.AllErrors;
            }

            List<FunctionParameter> effectiveParameters =
                new List<FunctionParameter> { new FunctionParameter("self", structType) };
            effectiveParameters.AddRange(parameters);

            ILChunks.GenerateFunctionEnter(syncBlockContext, structType.Name + "." + name, effectiveParameters);

            if (isAsync) {
                asyncClass ??= AsyncClass.Create(syncBlockContext, structType.Name.ToUpper() + "_" + name, declaredReturn, effectiveParameters,
                    expression);

                for (int idx = 0; idx < effectiveParameters.Count; idx++)
                    MethodParameter.EmitLoadArg(syncBlockContext.IL, idx);
                syncBlockContext.IL.EmitNew(OpCodes.Newobj, asyncClass.Value.constructor, effectiveParameters.Count);
                syncBlockContext.IL.EmitReturn(asyncClass.Value.type);

                return Enumerable.Empty<StructuralError>();
            }

            expression.EmitCode(syncBlockContext, declaredReturn == BuiltinType.Unit);

            if (!syncBlockContext.HasErrors && declaredReturn != BuiltinType.Unit)
                declaredReturn.AssignFrom(syncBlockContext.ModuleContext, expression.ResultType(syncBlockContext))
                    .EmitConvert(syncBlockContext);
            else if (declaredReturn == BuiltinType.Unit) {
                syncBlockContext.IL.Emit(OpCodes.Ldnull);
            }

            ILChunks.GenerateFunctionLeave(syncBlockContext);
            syncBlockContext.IL.EmitReturn(syncBlockContext.MethodBuilder.ReturnType);

            return syncBlockContext.AllErrors;
        }

        public override REPLValueFuture Eval(REPLContext context) {
            throw new REPLException(this, "Not supported in REPL mode");
        }
    }
}
