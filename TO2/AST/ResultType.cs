using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ResultType : RealizedType {
        public readonly TO2Type successType;
        public readonly TO2Type errorType;
        private Type generatedType;
        private readonly OperatorCollection allowedSuffixOperators;
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public ResultType(TO2Type successType, TO2Type errorType) {
            this.successType = successType;
            this.errorType = errorType;
            allowedSuffixOperators = new OperatorCollection {
                {Operator.Unwrap, new ResultUnwrapOperator(this)}
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {"success", new ResultFieldAccess(this, ResultField.Success)},
                {"value", new ResultFieldAccess(this, ResultField.Value)},
                {"error", new ResultFieldAccess(this, ResultField.Error)}
            };
        }

        public override string Name => $"Result<{successType}, {errorType}>";

        public override bool IsValid(ModuleContext context) =>
            successType.IsValid(context) && errorType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) =>
            new ResultType(successType.UnderlyingType(context), errorType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ??= DeriveType(context);

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override IUnapplyEmitter
            AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) {
            switch (unapplyName) {
            case "Ok" when itemCount == 1: return new ResultOkUnapplyEmitter(this);
            case "Err" when itemCount == 1: return new ResultErrUnapplyEmitter(this);
            default: return null;
            }
        }

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            if (otherType.UnderlyingType(context) is ResultType otherResultType)
                return successType.IsAssignableFrom(context, otherResultType.successType) &&
                       errorType.IsAssignableFrom(context, otherResultType.errorType);

            return successType.IsAssignableFrom(context, otherType);
        }

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            RealizedType underlyingOther = otherType.UnderlyingType(context);
            return !(underlyingOther is ResultType) && successType.IsAssignableFrom(context, underlyingOther)
                ? new AssignOk(this, otherType)
                : DefaultAssignEmitter.Instance;
        }

        public override RealizedType
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => new ResultType(
            successType.UnderlyingType(context).FillGenerics(context, typeArguments),
            errorType.UnderlyingType(context).FillGenerics(context, typeArguments));

        private Type DeriveType(ModuleContext context) =>
            typeof(Result<,>).MakeGenericType(successType.GeneratedType(context), errorType.GeneratedType(context));

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
            RealizedType concreteType) {
            ResultType concreteResult = concreteType as ResultType;
            if (concreteResult == null) return Enumerable.Empty<(string name, RealizedType type)>();
            return successType.InferGenericArgument(context, concreteResult.successType.UnderlyingType(context)).Concat(
                errorType.InferGenericArgument(context, concreteResult.errorType.UnderlyingType(context)));
        }
    }

    internal enum ResultField {
        Success,
        Value,
        Error,
    }

    internal class ResultFieldAccess : IFieldAccessFactory {
        private readonly ResultType resultType;
        private readonly ResultField field;

        internal ResultFieldAccess(ResultType resultType, ResultField field) {
            this.resultType = resultType;
            this.field = field;
        }

        public TO2Type DeclaredType {
            get {
                switch (field) {
                case ResultField.Success: return BuiltinType.Bool;
                case ResultField.Value: return resultType.successType;
                case ResultField.Error: return resultType.errorType;
                default: throw new InvalidOperationException($"Unknown option field: {field}");
                }
            }
        }

        public string Description {
            get {
                switch (field) {
                case ResultField.Success: return "`true` if the operation was successful";
                case ResultField.Value: return "Successful result of the operation";
                case ResultField.Error: return "Error result of the operation";
                default: throw new InvalidOperationException($"Unknown option field: {field}");
                }
            }
        }

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = resultType.GeneratedType(context);
            switch (field) {
            case ResultField.Success:
                return new BoundFieldAccessEmitter(BuiltinType.Bool, generateType,
                    new List<FieldInfo> { generateType.GetField("success") });
            case ResultField.Value:
                return new BoundFieldAccessEmitter(resultType.successType.UnderlyingType(context), generateType,
                    new List<FieldInfo> { generateType.GetField("value") });
            case ResultField.Error:
                return new BoundFieldAccessEmitter(resultType.errorType.UnderlyingType(context), generateType,
                    new List<FieldInfo> { generateType.GetField("error") });
            default: throw new InvalidOperationException($"Unknown option field: {field}");
            }
        }

        public IFieldAccessFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class AssignOk : IAssignEmitter {
        private readonly ResultType resultType;
        private readonly TO2Type otherType;

        internal AssignOk(ResultType resultType, TO2Type otherType) {
            this.resultType = resultType;
            this.otherType = otherType;
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            using ITempBlockVariable valueTemp =
                context.MakeTempVariable(resultType.successType.UnderlyingType(context.ModuleContext));
            resultType.successType.AssignFrom(context.ModuleContext, otherType)
                .EmitAssign(context, valueTemp, expression, true);

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            valueTemp.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            if (!dropResult) variable.EmitLoad(context);
        }

        public void EmitConvert(IBlockContext context) {
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            using ITempLocalRef value =
                context.IL.TempLocal(resultType.successType.GeneratedType(context.ModuleContext));
            resultType.successType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context);
            value.EmitStore(context);

            using ITempLocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("success"));
            value.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            someResult.EmitLoad(context);
        }
    }

    internal class ResultUnwrapOperator : IOperatorEmitter {
        private readonly ResultType resultType;

        internal ResultUnwrapOperator(ResultType resultType) => this.resultType = resultType;

        public bool Accepts(ModuleContext context, TO2Type otherType) => otherType == BuiltinType.Unit;

        public TO2Type OtherType => BuiltinType.Unit;

        public TO2Type ResultType => resultType.successType;

        public void EmitCode(IBlockContext context, Node target) {
            ResultType expectedReturn = context.ExpectedReturn.UnderlyingType(context.ModuleContext) as ResultType;
            if (expectedReturn == null ||
                !expectedReturn.errorType.IsAssignableFrom(context.ModuleContext, resultType.errorType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Operator ? is only allowed if function returns a result with error type {resultType.errorType}",
                    target.Start,
                    target.End
                ));
                return;
            }

            // Take success
            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));

            LabelRef onSuccess = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
            // Keep track of stuff that is still on the stack at onSuccess
            int stackAdjust = context.IL.StackCount;
            using (ITempLocalRef tempError =
                context.IL.TempLocal(expectedReturn.errorType.GeneratedType(context.ModuleContext))) {
                Type errorResultType = expectedReturn.GeneratedType(context.ModuleContext);
                using (ITempLocalRef errorResult = context.IL.TempLocal(errorResultType)) {
                    context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("error"));
                    tempError.EmitStore(context);
                    // Clean stack entirely to make room for error result to return
                    for (int i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
                    errorResult.EmitLoadPtr(context);
                    context.IL.Emit(OpCodes.Dup);
                    context.IL.Emit(OpCodes.Initobj, errorResultType, 1, 0);
                    context.IL.Emit(OpCodes.Dup);
                    context.IL.Emit(OpCodes.Ldc_I4_0);
                    context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("success"));
                    tempError.EmitLoad(context);
                    context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("error"));
                    errorResult.EmitLoad(context);
                    if (context.IsAsync) {
                        context.IL.EmitNew(OpCodes.Newobj,
                            context.MethodBuilder.ReturnType.GetConstructor(new[] { errorResultType }));
                    }

                    context.IL.EmitReturn(context.MethodBuilder.ReturnType);
                }
            }

            context.IL.MarkLabel(onSuccess);

            // Readjust the stack counter
            context.IL.AdjustStack(stackAdjust);
            // Get success value if necessary or drop result
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;
    }

    internal class ResultOkUnapplyEmitter : IUnapplyEmitter {
        private readonly ResultType resultType;

        public string Name => "Ok";
        public List<TO2Type> Items { get; }

        internal ResultOkUnapplyEmitter(ResultType resultType) {
            this.resultType = resultType;
            Items = new List<TO2Type> { resultType.successType };
        }

        public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
            IBlockVariable target = targetVariables[0];

            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));

            if (target == null) {
                context.IL.Emit(OpCodes.Pop);
            } else {
                LabelRef onError = context.IL.DefineLabel(true);
                LabelRef end = context.IL.DefineLabel(true);

                context.IL.Emit(OpCodes.Brfalse_S, onError);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
                target.EmitStore(context);
                context.IL.Emit(OpCodes.Ldc_I4_1);
                context.IL.Emit(OpCodes.Br_S, end);

                context.IL.MarkLabel(onError);
                context.IL.Emit(OpCodes.Pop);
                context.IL.Emit(OpCodes.Ldc_I4_0);

                context.IL.MarkLabel(end);
            }
        }
    }

    internal class ResultErrUnapplyEmitter : IUnapplyEmitter {
        private readonly ResultType resultType;

        public string Name => "Err";
        public List<TO2Type> Items { get; }

        internal ResultErrUnapplyEmitter(ResultType resultType) {
            this.resultType = resultType;
            Items = new List<TO2Type> { resultType.errorType };
        }

        public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
            IBlockVariable target = targetVariables[0];

            Type generatedType = resultType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));
            if (target == null) {
                context.IL.Emit(OpCodes.Ldc_I4_0);
                context.IL.Emit(OpCodes.Ceq);
                context.IL.Emit(OpCodes.Pop);
            } else {
                LabelRef onOk = context.IL.DefineLabel(true);
                LabelRef end = context.IL.DefineLabel(true);

                context.IL.Emit(OpCodes.Brtrue_S, onOk);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("error"));
                target.EmitStore(context);
                context.IL.Emit(OpCodes.Ldc_I4_1);
                context.IL.Emit(OpCodes.Br_S, end);

                context.IL.MarkLabel(onOk);
                context.IL.Emit(OpCodes.Pop);
                context.IL.Emit(OpCodes.Ldc_I4_0);

                context.IL.MarkLabel(end);
            }
        }
    }
}
