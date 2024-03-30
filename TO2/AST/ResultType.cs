using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ResultType : RealizedType {
    private readonly OperatorCollection allowedSuffixOperators;
    public readonly TO2Type successType;

    public ResultType(TO2Type successType) {
        this.successType = successType;
        allowedSuffixOperators = new OperatorCollection {
            { Operator.Unwrap, new ResultUnwrapOperator(this) }
        };
        DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
            { "success", new ResultFieldAccess(this, ResultField.Success) },
            { "value", new ResultFieldAccess(this, ResultField.Value) },
            { "error", new ResultFieldAccess(this, ResultField.Error) }
        };
    }

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override string Name => $"Result<{successType}>";

    public override string LocalName => "Result";

    public override bool IsValid(ModuleContext context) => successType.IsValid(context);

    public override RealizedType UnderlyingType(ModuleContext context) =>
        new ResultType(successType.UnderlyingType(context));

    public override Type GeneratedType(ModuleContext context) => DeriveType(context);

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

    public override IUnapplyEmitter?
        AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) {
        return unapplyName switch {
            "Ok" when itemCount == 1 => new ResultOkUnapplyEmitter(this),
            "Err" when itemCount == 1 => new ResultErrUnapplyEmitter(this),
            _ => null,
        };
    }

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        if (otherType.UnderlyingType(context) is ResultType otherResultType)
            return successType == otherResultType.successType ||
                   successType.IsAssignableFrom(context, otherResultType.successType);

        return successType == otherType || successType.IsAssignableFrom(context, otherType);
    }

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        var underlyingOther = otherType.UnderlyingType(context);
        return underlyingOther is not ResultType && successType.IsAssignableFrom(context, underlyingOther)
            ? new AssignOk(this, otherType)
            : DefaultAssignEmitter.Instance;
    }

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        return new ResultType(
            successType.UnderlyingType(context).FillGenerics(context, typeArguments));
    }

    private Type DeriveType(ModuleContext context) {
        return typeof(Result<>).MakeGenericType(successType.GeneratedType(context));
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        if (concreteType is not ResultType concreteResult) return [];
        return successType.InferGenericArgument(context, concreteResult.successType.UnderlyingType(context));
    }
}

internal enum ResultField {
    Success,
    Value,
    Error
}

internal class ResultFieldAccess : IFieldAccessFactory {
    private readonly ResultField field;
    private readonly ResultType resultType;

    internal ResultFieldAccess(ResultType resultType, ResultField field) {
        this.resultType = resultType;
        this.field = field;
    }

    public TO2Type DeclaredType {
        get {
            return field switch {
                ResultField.Success => BuiltinType.Bool,
                ResultField.Value => resultType.successType,
                ResultField.Error => BuiltinType.Error,
                _ => throw new InvalidOperationException($"Unknown option field: {field}"),
            };
        }
    }

    public string Description {
        get {
            return field switch {
                ResultField.Success => "`true` if the operation was successful",
                ResultField.Value => "Successful result of the operation",
                ResultField.Error => "Error result of the operation",
                _ => throw new InvalidOperationException($"Unknown option field: {field}"),
            };
        }
    }

    public bool CanStore => false;

    public IFieldAccessEmitter Create(ModuleContext context) {
        var generateType = resultType.GeneratedType(context);
        return field switch {
            ResultField.Success => new BoundFieldAccessEmitter(BuiltinType.Bool, generateType,
                [generateType.GetField("success")]),
            ResultField.Value => new BoundFieldAccessEmitter(resultType.successType.UnderlyingType(context),
                generateType,
                [generateType.GetField("value")]),
            ResultField.Error => new BoundFieldAccessEmitter(BuiltinType.Error.UnderlyingType(context), generateType,
                [generateType.GetField("error")]),
            _ => throw new InvalidOperationException($"Unknown option field: {field}"),
        };
    }

    public IFieldAccessFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

internal class AssignOk : IAssignEmitter {
    private readonly TO2Type otherType;
    private readonly ResultType resultType;

    internal AssignOk(ResultType resultType, TO2Type otherType) {
        this.resultType = resultType;
        this.otherType = otherType;
    }

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
        var generatedType = resultType.GeneratedType(context.ModuleContext);
        using var valueTemp =
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

    public void EmitConvert(IBlockContext context, bool mutableTarget) {
        var generatedType = resultType.GeneratedType(context.ModuleContext);
        using var value =
            context.IL.TempLocal(resultType.successType.GeneratedType(context.ModuleContext));
        resultType.successType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context, mutableTarget);
        value.EmitStore(context);

        using var someResult = context.IL.TempLocal(generatedType);
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

    public IREPLValue EvalConvert(Node node, IREPLValue value) {
        if (value.Type == resultType) return value;

        return new REPLAny(resultType, Result.Ok<object>(resultType.successType.REPLCast(value.Value)));
    }
}

internal class ResultUnwrapOperator : IOperatorEmitter {
    private readonly ResultType resultType;

    internal ResultUnwrapOperator(ResultType resultType) {
        this.resultType = resultType;
    }

    public TO2Type OtherType => BuiltinType.Unit;

    public TO2Type ResultType => resultType.successType;

    public bool Accepts(ModuleContext context, TO2Type otherType) => otherType == BuiltinType.Unit;

    public void EmitCode(IBlockContext context, Node target) {
        if (context.ExpectedReturn.UnderlyingType(context.ModuleContext) is not ResultType expectedReturn) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Operator ? is only allowed if function returns a result",
                target.Start,
                target.End
            ));
            return;
        }

        // Take success
        var generatedType = resultType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));

        var onSuccess = context.IL.DefineLabel(true);

        context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
        // Keep track of stuff that is still on the stack at onSuccess
        var stackAdjust = context.IL.StackCount;
        using (var tempError =
               context.IL.TempLocal(BuiltinType.Error.GeneratedType(context.ModuleContext))) {
            var errorResultType = expectedReturn.GeneratedType(context.ModuleContext);

            using var errorResult = context.IL.TempLocal(errorResultType);
            context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("error"));
            tempError.EmitStore(context);
            // Clean stack entirely to make room for error result to return
            for (var i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
            errorResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, errorResultType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_0);
            context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("success"));
            tempError.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, errorResultType.GetField("error"));
            errorResult.EmitLoad(context);
            if (context.IsAsync)
                context.IL.EmitNew(OpCodes.Newobj,
                    context.MethodBuilder!.ReturnType.GetConstructor([errorResultType])!);

            ILChunks.GenerateFunctionLeave(context);
            context.IL.EmitReturn(context.MethodBuilder!.ReturnType);
        }

        context.IL.MarkLabel(onSuccess);

        // Readjust the stack counter
        context.IL.AdjustStack(stackAdjust);
        // Get success value if necessary or drop result
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
    }

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
        EmitCode(context, target);
        variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context, !variable.IsConst);
        variable.EmitStore(context);
    }

    public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;

    public IREPLValue Eval(Node node, IREPLValue left, IREPLValue? right) {
        if (left.Type is ResultType lrt && left.Value is IAnyResult lr)
            return lr.Success
                ? lrt.successType.REPLCast(lr.ValueObject)
                : new REPLReturn(BuiltinType.Error.REPLCast(lr.ErrorObject));

        throw new REPLException(node, $"Expected {left.Type} to be a result");
    }
}

internal class ResultOkUnapplyEmitter : IUnapplyEmitter {
    private readonly ResultType resultType;

    internal ResultOkUnapplyEmitter(ResultType resultType) {
        this.resultType = resultType;
        Items = [resultType.successType];
    }

    public string Name => "Ok";
    public List<TO2Type> Items { get; }

    public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
        var target = targetVariables[0];

        var generatedType = resultType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));

        if (target == null) {
            context.IL.Emit(OpCodes.Pop);
        } else {
            var onError = context.IL.DefineLabel(true);
            var end = context.IL.DefineLabel(true);

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

    internal ResultErrUnapplyEmitter(ResultType resultType) {
        this.resultType = resultType;
        Items = [BuiltinType.Error];
    }

    public string Name => "Err";
    public List<TO2Type> Items { get; }

    public void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables) {
        var target = targetVariables[0];

        var generatedType = resultType.GeneratedType(context.ModuleContext);
        context.IL.Emit(OpCodes.Dup);
        context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("success"));
        if (target == null) {
            context.IL.Emit(OpCodes.Ldc_I4_0);
            context.IL.Emit(OpCodes.Ceq);
            context.IL.Emit(OpCodes.Pop);
        } else {
            var onOk = context.IL.DefineLabel(true);
            var end = context.IL.DefineLabel(true);

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
