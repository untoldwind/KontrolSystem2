using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;
using Option = KontrolSystem.Parsing.Option;

namespace KontrolSystem.TO2.AST;

public class OptionType : RealizedType {
    private readonly OperatorCollection allowedSuffixOperators;
    public readonly TO2Type elementType;

    public OptionType(TO2Type elementType) {
        this.elementType = elementType;
        allowedSuffixOperators = new OperatorCollection {
            { Operator.BitOr, new OptionBitOrOperator(this) },
            { Operator.Unwrap, new OptionUnwrapOperator(this) }
        };
        DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
            { "map", new OptionMapFactory(this) },
            { "then", new OptionThenFactory(this) },
            { "ok_or", new OptionOkOrFactory(this) }
        };
        DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
            { "defined", new OptionFieldAccess(this, OptionField.Defined) },
            { "value", new OptionFieldAccess(this, OptionField.Value) }
        };
    }

    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override string Name => $"Option<{elementType}>";

    public override string LocalName => "Option";

    public override bool IsValid(ModuleContext context) {
        return elementType.IsValid(context);
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return new OptionType(elementType.UnderlyingType(context));
    }

    public override Type GeneratedType(ModuleContext context) {
        return DeriveType(context);
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
        return allowedSuffixOperators;
    }

    public override IUnapplyEmitter?
        AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) {
        switch (unapplyName) {
        case "Some" when itemCount == 1: return new OptionSomeUnapplyEmitter(this);
        default: return null;
        }
    }

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        if (otherType.UnderlyingType(context) is OptionType otherOption)
            return elementType == otherOption.elementType ||
                   elementType.IsAssignableFrom(context, otherOption.elementType);

        return elementType == otherType || elementType.IsAssignableFrom(context, otherType);
    }

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        var underlyingOther = otherType.UnderlyingType(context);

        return !(underlyingOther is OptionType) && elementType.IsAssignableFrom(context, underlyingOther)
            ? new AssignSome(this, otherType)
            : DefaultAssignEmitter.Instance;
    }

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        return new OptionType(elementType.UnderlyingType(context).FillGenerics(context, typeArguments));
    }

    private Type DeriveType(ModuleContext context) {
        return typeof(Option<>).MakeGenericType(elementType.GeneratedType(context));
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        var concreteOption = concreteType as OptionType;
        if (concreteOption == null) return Enumerable.Empty<(string name, RealizedType type)>();
        return elementType.InferGenericArgument(context, concreteOption.elementType.UnderlyingType(context));
    }
}

internal enum OptionField {
    Defined,
    Value
}

internal class OptionFieldAccess : IFieldAccessFactory {
    private readonly OptionField field;
    private readonly OptionType optionType;

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

    public bool CanStore => false;

    public IFieldAccessEmitter Create(ModuleContext context) {
        var generateType = optionType.GeneratedType(context);
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
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

internal class AssignSome : IAssignEmitter {
    private readonly OptionType optionType;
    private readonly TO2Type otherType;

    internal AssignSome(OptionType optionType, TO2Type otherType) {
        this.optionType = optionType;
        this.otherType = otherType;
    }

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
        var generatedType = optionType.GeneratedType(context.ModuleContext);
        using var valueTemp =
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
        var generatedType = optionType.GeneratedType(context.ModuleContext);
        using var value =
            context.IL.TempLocal(optionType.elementType.GeneratedType(context.ModuleContext));
        optionType.elementType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context);
        value.EmitStore(context);
        using var someResult = context.IL.TempLocal(generatedType);
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

    public IREPLValue EvalConvert(Node node, IREPLValue value) {
        if (value.Type == optionType) return value;

        return new REPLAny(optionType, Option.Some(optionType.elementType.REPLCast(value.Value)));
    }
}

internal class OptionBitOrOperator : IOperatorEmitter {
    private readonly OptionType optionType;

    internal OptionBitOrOperator(OptionType optionType) {
        this.optionType = optionType;
    }

    public bool Accepts(ModuleContext context, TO2Type otherType) {
        return optionType.elementType.IsAssignableFrom(context, otherType);
    }

    public TO2Type OtherType => optionType.elementType;

    public TO2Type ResultType => optionType.elementType;

    public void EmitCode(IBlockContext context, Node target) {
        using var tempDefault =
            context.MakeTempVariable(optionType.elementType.UnderlyingType(context.ModuleContext));

        tempDefault.EmitStore(context);

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));
        var end = context.IL.DefineLabel(true);
        var onUndefined = context.IL.DefineLabel(true);

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

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));
        var end = context.IL.DefineLabel(true);
        var onUndefined = context.IL.DefineLabel(true);

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

    public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }

    public IREPLValue Eval(Node node, IREPLValue left, IREPLValue? right) {
        if (left.Type is OptionType lot && left.Value is IAnyOption lo)
            return lot.elementType.REPLCast(lo.Defined ? lo.ValueObject : right);

        throw new REPLException(node, $"Expected {left.Type} to be an option");
    }
}

internal class OptionUnwrapOperator : IOperatorEmitter {
    private readonly OptionType optionType;

    internal OptionUnwrapOperator(OptionType optionType) {
        this.optionType = optionType;
    }

    public bool Accepts(ModuleContext context, TO2Type otherType) {
        return otherType == BuiltinType.Unit;
    }

    public TO2Type OtherType => BuiltinType.Unit;

    public TO2Type ResultType => optionType.elementType;

    public void EmitCode(IBlockContext context, Node target) {
        var expectedReturn = context.ExpectedReturn.UnderlyingType(context.ModuleContext) as OptionType;
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
        var generatedType = optionType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));

        var onSuccess = context.IL.DefineLabel(true);

        context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
        // Keep track of stuff that is still on the stack at onSuccess
        var stackAdjust = context.IL.StackCount;
        var noneType = expectedReturn.GeneratedType(context.ModuleContext);
        using (var noneResult = context.IL.TempLocal(noneType)) {
            // Clean stack entirely to make room for error result to return
            for (var i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
            noneResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            noneResult.EmitLoad(context);
            if (context.IsAsync)
                context.IL.EmitNew(OpCodes.Newobj,
                    context.MethodBuilder!.ReturnType.GetConstructor(new[] { noneType })!);

            ILChunks.GenerateFunctionLeave(context);
            context.IL.EmitReturn(context.MethodBuilder!.ReturnType);

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

    public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }

    public IREPLValue Eval(Node node, IREPLValue left, IREPLValue? right) {
        if (left.Type is OptionType lot && left.Value is IAnyOption lo)
            return lo.Defined
                ? lot.elementType.REPLCast(lo.ValueObject)
                : new REPLReturn(new REPLAny(lot, Option.None<object>()));

        throw new REPLException(node, $"Expected {left.Type} to be an option");
    }
}

internal class OptionMapFactory : IMethodInvokeFactory {
    private readonly OptionType optionType;

    internal OptionMapFactory(OptionType optionType) {
        this.optionType = optionType;
    }

    public TypeHint? ReturnHint => null;

    public TypeHint ArgumentHint(int argumentIdx) {
        return _ =>
            argumentIdx == 0
                ? new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit)
                : null;
    }

    public string Description => "Map the content of the option";

    public bool IsAsync => false;

    public bool IsConst => true;

    public TO2Type DeclaredReturn => new OptionType(BuiltinType.Unit);

    public List<FunctionParameter> DeclaredParameters => new() {
        new("mapper",
            new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit),
            "Function to be applied on the optional value if defined")
    };

    public IMethodInvokeEmitter? Create(IBlockContext context, List<TO2Type> arguments, Node node) {
        if (arguments.Count != 1) return null;
        var mapper = arguments[0].UnderlyingType(context.ModuleContext) as FunctionType;
        if (mapper == null) return null;

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        var methodInfo =
            generatedType.GetMethod("Map")?
                .MakeGenericMethod(mapper.returnType.GeneratedType(context.ModuleContext)) ??
            throw new ArgumentException($"No Map method in {generatedType}");

        return new BoundMethodInvokeEmitter(new OptionType(mapper.returnType),
            new List<RealizedParameter> {
                new("mapper", mapper, "Function to be applied on the optional value if defined")
            }, false, generatedType,
            methodInfo);
    }

    public IMethodInvokeFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

internal class OptionThenFactory : IMethodInvokeFactory {
    private readonly OptionType optionType;

    internal OptionThenFactory(OptionType optionType) {
        this.optionType = optionType;
    }

    public TypeHint? ReturnHint => null;

    public TypeHint ArgumentHint(int argumentIdx) {
        return _ =>
            argumentIdx == 0
                ? new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit)
                : null;
    }

    public string Description =>
        "Continue with a second operation that also has an optional result. (Also called flat_map)";

    public bool IsAsync => false;

    public bool IsConst => true;

    public TO2Type DeclaredReturn => new OptionType(BuiltinType.Unit);

    public List<FunctionParameter> DeclaredParameters => new() {
        new("mapper",
            new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuiltinType.Unit),
            "Function to be applied on the optional value if defined")
    };

    public IMethodInvokeEmitter? Create(IBlockContext context, List<TO2Type> arguments, Node node) {
        if (arguments.Count != 1) return null;
        var mapper = arguments[0].UnderlyingType(context.ModuleContext) as FunctionType;
        if (mapper == null) return null;

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        var mapperReturnType = mapper.returnType.GeneratedType(context.ModuleContext);

        if (!mapperReturnType.IsGenericType || mapperReturnType.GetGenericTypeDefinition() != typeof(Option<>)) {
            context.AddError(new StructuralError(StructuralError.ErrorType.InvalidType,
                "Return value of then is not an Option", node.Start, node.End));
            return null;
        }

        var methodInfo =
            generatedType.GetMethod("Then")?.MakeGenericMethod(mapperReturnType.GenericTypeArguments[0]) ??
            throw new ArgumentException($"No Then method in {generatedType}");

        return new BoundMethodInvokeEmitter(mapper.returnType.UnderlyingType(context.ModuleContext),
            new List<RealizedParameter> {
                new("mapper", mapper, "Function to be applied on the optional value if defined")
            }, false, generatedType,
            methodInfo);
    }

    public IMethodInvokeFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

internal class OptionOkOrFactory : IMethodInvokeFactory {
    private readonly OptionType optionType;

    internal OptionOkOrFactory(OptionType optionType) {
        this.optionType = optionType;
    }

    public TypeHint ReturnHint => _ => new ResultType(optionType.elementType, BuiltinType.Unit);

    public string Description => "Convert the option to a result, where None is mapped to the `if_none` error";

    public bool IsAsync => false;

    public bool IsConst => true;

    public TypeHint? ArgumentHint(int argumentIdx) {
        return null;
    }

    public TO2Type DeclaredReturn => new ResultType(optionType.elementType, BuiltinType.Unit);

    public List<FunctionParameter> DeclaredParameters => new()
        { new("if_none", BuiltinType.Unit, "Get error message if option is undefined") };

    public IMethodInvokeEmitter? Create(IBlockContext context, List<TO2Type> arguments, Node node) {
        if (arguments.Count != 1) return null;
        var errorType = arguments[0].UnderlyingType(context.ModuleContext);

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        var methodInfo = generatedType.GetMethod("OkOr")?
                             .MakeGenericMethod(errorType.GeneratedType(context.ModuleContext)) ??
                         throw new ArgumentException($"No OkOr method in {generatedType}");


        return new BoundMethodInvokeEmitter(new ResultType(optionType.elementType, errorType),
            new List<RealizedParameter> {
                new("if_none", errorType, "Get error message if option is undefined")
            }, false, generatedType,
            methodInfo);
    }

    public IMethodInvokeFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

internal class OptionSomeUnapplyEmitter : IUnapplyEmitter {
    private readonly OptionType optionType;

    internal OptionSomeUnapplyEmitter(OptionType optionType) {
        this.optionType = optionType;
        Items = new List<TO2Type> { optionType.elementType };
    }

    public string Name => "Some";
    public List<TO2Type> Items { get; }

    public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
        var target = targetVariables[0];

        var generatedType = optionType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));

        if (target == null) {
            context.IL.Emit(OpCodes.Pop);
        } else {
            var onUndefined = context.IL.DefineLabel(true);
            var end = context.IL.DefineLabel(true);

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
