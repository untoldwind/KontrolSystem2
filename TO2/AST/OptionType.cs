using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class OptionType : RealizedType {
        public readonly TO2Type elementType;
        private Type generatedType;
        private readonly OperatorCollection allowedSuffixOperators;
        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public OptionType(TO2Type elementType) {
            this.elementType = elementType;
            allowedSuffixOperators = new OperatorCollection {
                {Operator.BitOr, new OptionBitOrOperator(this)},
                {Operator.Unwrap, new OptionUnwrapOperator(this)}
            };
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"map", new OptionMapFactory(this)},
                {"then", new OptionThenFactory(this)},
                {"ok_or", new OptionOkOrFactory(this)}
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {"defined", new OptionFieldAccess(this, OptionField.Defined)},
                {"value", new OptionFieldAccess(this, OptionField.Value)}
            };
        }

        public override string Name => $"Option<{elementType}>";

        public override bool IsValid(ModuleContext context) => elementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) =>
            new OptionType(elementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ??= DeriveType(context);

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override IUnapplyEmitter
            AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) {
            switch (unapplyName) {
            case "Some" when itemCount == 1: return new OptionSomeUnapplyEmitter(this);
            default: return null;
            }
        }

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            if (otherType.UnderlyingType(context) is OptionType otherOption)
                return elementType.IsAssignableFrom(context, otherOption.elementType);

            return elementType.IsAssignableFrom(context, otherType);
        }

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            RealizedType underlyingOther = otherType.UnderlyingType(context);

            return !(underlyingOther is OptionType) && elementType.IsAssignableFrom(context, underlyingOther)
                ? new AssignSome(this, otherType)
                : DefaultAssignEmitter.Instance;
        }

        public override RealizedType
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            new OptionType(elementType.UnderlyingType(context).FillGenerics(context, typeArguments));

        private Type DeriveType(ModuleContext context) =>
            typeof(Option<>).MakeGenericType(elementType.GeneratedType(context));

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
            RealizedType concreteType) {
            OptionType concreteOption = concreteType as OptionType;
            if (concreteOption == null) return Enumerable.Empty<(string name, RealizedType type)>();
            return elementType.InferGenericArgument(context, concreteOption.elementType.UnderlyingType(context));
        }
    }

    internal enum OptionField {
        Defined,
        Value,
    }

    internal class OptionFieldAccess : IFieldAccessFactory {
        private readonly OptionType optionType;
        private readonly OptionField field;

        internal OptionFieldAccess(OptionType optionType, OptionField field) {
            this.optionType = optionType;
            this.field = field;
        }

        public TO2Type DeclaredType {
            get {
                switch (field) {
                case OptionField.Defined: return BuiltinType.Bool;
                case OptionField.Value: return optionType.elementType;
                default: throw new InvalidOperationException($"Unknown option field: {field}");
                }
            }
        }

        public string Description {
            get {
                switch (field) {
                case OptionField.Defined: return "`true` if the option is defined, i.e. contains a value";
                case OptionField.Value: return "Value of the option if defined";
                default: throw new InvalidOperationException($"Unknown option field: {field}");
                }
            }
        }

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = optionType.GeneratedType(context);
            switch (field) {
            case OptionField.Defined:
                return new BoundFieldAccessEmitter(BuiltinType.Bool, generateType,
                    new List<FieldInfo> { generateType.GetField("defined") });
            case OptionField.Value:
                return new BoundFieldAccessEmitter(optionType.elementType.UnderlyingType(context), generateType,
                    new List<FieldInfo> { generateType.GetField("value") });
            default: throw new InvalidOperationException($"Unknown option field: {field}");
            }
        }

        public IFieldAccessFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class AssignSome : IAssignEmitter {
        private readonly OptionType optionType;
        private readonly TO2Type otherType;

        internal AssignSome(OptionType optionType, TO2Type otherType) {
            this.optionType = optionType;
            this.otherType = otherType;
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            using ITempBlockVariable valueTemp =
                context.MakeTempVariable(optionType.elementType.UnderlyingType(context.ModuleContext));
            optionType.elementType.AssignFrom(context.ModuleContext, otherType)
                .EmitAssign(context, valueTemp, expression, true);

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            valueTemp.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            if (!dropResult) variable.EmitLoad(context);
        }

        public void EmitConvert(IBlockContext context) {
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            using ITempLocalRef value =
                context.IL.TempLocal(optionType.elementType.GeneratedType(context.ModuleContext));
            optionType.elementType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context);
            value.EmitStore(context);
            using ITempLocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            value.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            someResult.EmitLoad(context);
        }
    }

    internal class OptionBitOrOperator : IOperatorEmitter {
        private readonly OptionType optionType;

        internal OptionBitOrOperator(OptionType optionType) => this.optionType = optionType;

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            optionType.elementType.IsAssignableFrom(context, otherType);

        public TO2Type OtherType => optionType.elementType;

        public TO2Type ResultType => optionType.elementType;

        public void EmitCode(IBlockContext context, Node target) {
            using ITempBlockVariable tempDefault =
                context.MakeTempVariable(optionType.elementType.UnderlyingType(context.ModuleContext));

            tempDefault.EmitStore(context);

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));
            LabelRef end = context.IL.DefineLabel(true);
            LabelRef onUndefined = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brfalse_S, onUndefined);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
            context.IL.Emit(OpCodes.Br_S, end);

            context.IL.MarkLabel(onUndefined);
            context.IL.Emit(OpCodes.Pop);
            tempDefault.EmitLoad(context);

            context.IL.MarkLabel(end);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));
            LabelRef end = context.IL.DefineLabel(true);
            LabelRef onUndefined = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brfalse_S, onUndefined);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
            context.IL.Emit(OpCodes.Br_S, end);

            context.IL.MarkLabel(onUndefined);
            context.IL.Emit(OpCodes.Pop);

            context.IL.MarkLabel(end);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;
    }

    internal class OptionUnwrapOperator : IOperatorEmitter {
        private readonly OptionType optionType;

        internal OptionUnwrapOperator(OptionType optionType) => this.optionType = optionType;

        public bool Accepts(ModuleContext context, TO2Type otherType) => otherType == BuiltinType.Unit;

        public TO2Type OtherType => BuiltinType.Unit;

        public TO2Type ResultType => optionType.elementType;

        public void EmitCode(IBlockContext context, Node target) {
            OptionType expectedReturn = context.ExpectedReturn.UnderlyingType(context.ModuleContext) as OptionType;
            if (expectedReturn == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    "Operator ? is only allowed if function returns an option",
                    target.Start,
                    target.End
                ));
                return;
            }

            // Take success
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));

            LabelRef onSuccess = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
            // Keep track of stuff that is still on the stack at onSuccess
            int stackAdjust = context.IL.StackCount;
            Type noneType = expectedReturn.GeneratedType(context.ModuleContext);
            using (ITempLocalRef noneResult = context.IL.TempLocal(noneType)) {
                // Clean stack entirely to make room for error result to return
                for (int i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
                noneResult.EmitLoadPtr(context);
                context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
                noneResult.EmitLoad(context);
                if (context.IsAsync) {
                    context.IL.EmitNew(OpCodes.Newobj,
                        context.MethodBuilder.ReturnType.GetConstructor(new[] { noneType }));
                }

                context.IL.EmitReturn(context.MethodBuilder.ReturnType);

                context.IL.MarkLabel(onSuccess);

                // Readjust the stack counter
                context.IL.AdjustStack(stackAdjust);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
            }
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;
    }

    internal class OptionMapFactory : IMethodInvokeFactory {
        private readonly OptionType optionType;

        internal OptionMapFactory(OptionType optionType) => this.optionType = optionType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ =>
            argumentIdx == 0
                ? new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit)
                : null;

        public string Description => "Map the content of the option";

        public bool IsConst => true;

        public TO2Type DeclaredReturn => new OptionType(BuiltinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> {
            new FunctionParameter("mapper",
                new FunctionType(false, new List<TO2Type> {optionType.elementType}, BuiltinType.Unit))
        };

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context.ModuleContext) as FunctionType;
            if (mapper == null) return null;

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            MethodInfo methodInfo =
                generatedType.GetMethod("Map")?
                    .MakeGenericMethod(mapper.returnType.GeneratedType(context.ModuleContext)) ??
                throw new ArgumentException($"No Map method in {generatedType}");

            return new BoundMethodInvokeEmitter(new OptionType(mapper.returnType),
                new List<RealizedParameter> { new RealizedParameter("mapper", mapper) }, false, generatedType,
                methodInfo);
        }

        public IMethodInvokeFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class OptionThenFactory : IMethodInvokeFactory {
        private readonly OptionType optionType;

        internal OptionThenFactory(OptionType optionType) => this.optionType = optionType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ =>
            argumentIdx == 0
                ? new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit)
                : null;

        public string Description =>
            "Continue with a second operation that also has an optional result. (Also called flat_map)";

        public bool IsConst => true;

        public TO2Type DeclaredReturn => new OptionType(BuiltinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> {
            new FunctionParameter("mapper",
                new FunctionType(false, new List<TO2Type> {optionType.elementType}, BuiltinType.Unit))
        };

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context.ModuleContext) as FunctionType;
            if (mapper == null) return null;

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            Type mapperReturnType = mapper.returnType.GeneratedType(context.ModuleContext);

            if (!mapperReturnType.IsGenericType || (mapperReturnType.GetGenericTypeDefinition() != typeof(Option<>))) {
                context.AddError(new StructuralError(StructuralError.ErrorType.InvalidType,
                    "Return value of then is not an Option", node.Start, node.End));
                return null;
            }

            MethodInfo methodInfo =
                generatedType.GetMethod("Then")?.MakeGenericMethod(mapperReturnType.GenericTypeArguments[0]) ??
                throw new ArgumentException($"No Then method in {generatedType}");

            return new BoundMethodInvokeEmitter(mapper.returnType.UnderlyingType(context.ModuleContext),
                new List<RealizedParameter> { new RealizedParameter("mapper", mapper) }, false, generatedType,
                methodInfo);
        }

        public IMethodInvokeFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class OptionOkOrFactory : IMethodInvokeFactory {
        private readonly OptionType optionType;

        internal OptionOkOrFactory(OptionType optionType) => this.optionType = optionType;

        public TypeHint ReturnHint => _ => new ResultType(optionType.elementType, BuiltinType.Unit);

        public string Description => "Convert the option to a result, where None is mapped to the `if_none` error";

        public bool IsConst => true;

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public TO2Type DeclaredReturn => new ResultType(optionType.elementType, BuiltinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter>
            {new FunctionParameter("on_error", BuiltinType.Unit)};

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) {
            if (arguments.Count != 1) return null;
            RealizedType errorType = arguments[0].UnderlyingType(context.ModuleContext);

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            MethodInfo methodInfo = generatedType.GetMethod("OkOr")?
                                        .MakeGenericMethod(errorType.GeneratedType(context.ModuleContext)) ??
                                    throw new ArgumentException($"No OkOr method in {generatedType}");


            return new BoundMethodInvokeEmitter(new ResultType(optionType.elementType, errorType),
                new List<RealizedParameter> { new RealizedParameter("if_none", errorType) }, false, generatedType,
                methodInfo);
        }

        public IMethodInvokeFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class OptionSomeUnapplyEmitter : IUnapplyEmitter {
        private readonly OptionType optionType;

        public string Name => "Some";
        public List<TO2Type> Items { get; }

        internal OptionSomeUnapplyEmitter(OptionType optionType) {
            this.optionType = optionType;
            Items = new List<TO2Type> { optionType.elementType };
        }

        public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
            IBlockVariable target = targetVariables[0];

            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));

            if (target == null) {
                context.IL.Emit(OpCodes.Pop);
            } else {
                LabelRef onUndefined = context.IL.DefineLabel(true);
                LabelRef end = context.IL.DefineLabel(true);

                context.IL.Emit(OpCodes.Brfalse_S, onUndefined);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
                target.EmitStore(context);
                context.IL.Emit(OpCodes.Ldc_I4_1);
                context.IL.Emit(OpCodes.Br_S, end);

                context.IL.MarkLabel(onUndefined);
                context.IL.Emit(OpCodes.Pop);
                context.IL.Emit(OpCodes.Ldc_I4_0);

                context.IL.MarkLabel(end);
            }
        }
    }
}
