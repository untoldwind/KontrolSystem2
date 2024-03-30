using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;
using Option = KontrolSystem.Parsing.Option;

namespace KontrolSystem.TO2.AST;

public class IfThen : Expression, IVariableContainer {
    private readonly Expression condition;
    private readonly Expression thenExpression;

    public IfThen(Expression condition, Expression thenExpression, Position start = new(),
        Position end = new()) : base(start, end) {
        this.condition = condition;
        this.condition.TypeHint = _ => BuiltinType.Bool;
        this.thenExpression = thenExpression;
    }

    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            condition.VariableContainer = value;
            thenExpression.VariableContainer = this;
        }
    }


    public override TypeHint? TypeHint {
        set {
            thenExpression.TypeHint = value != null ? context =>
                (value(context) as OptionType)?.elementType.UnderlyingType(context.ModuleContext) : null;
        }
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) => condition.GetScopeVariables(context)?.Get(name);

    public override TO2Type ResultType(IBlockContext context) => new OptionType(thenExpression.ResultType(context));

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (condition.ResultType(context) != BuiltinType.Bool) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    "Condition of if is not a boolean",
                    Start,
                    End
                )
            );
            return;
        }

        var thenContext = context.CreateChildContext();
        var scopeVariables = condition.GetScopeVariables(thenContext);

        if (scopeVariables != null)
            foreach (var (name, type) in scopeVariables.Select(x => (x.Key, x.Value))) {
                if (thenContext.FindVariable(name) != null) {
                    thenContext.AddError(new StructuralError(
                        StructuralError.ErrorType.DuplicateVariableName,
                        $"Variable '{name}' already declared in this scope",
                        Start,
                        End
                    ));
                    return;
                }

                thenContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
            }

        var thenCount = thenExpression.GetILCount(thenContext, true);

        if (!context.HasErrors && thenCount.stack > 0) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    "Then expression leaves values on stack. This must not happen",
                    Start,
                    End
                )
            );
            return;
        }

        condition.EmitCode(thenContext, false);

        if (context.HasErrors) return;

        var thenResultType = thenExpression.ResultType(thenContext);

        if (dropResult || thenResultType == BuiltinType.Unit) {
            var skipThen = context.IL.DefineLabel(thenCount.opCodes < 124);

            thenContext.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
            thenExpression.EmitCode(thenContext, true);
            thenContext.IL.MarkLabel(skipThen);
            if (!dropResult) {
                var noneType = new OptionType(BuiltinType.Unit).GeneratedType(thenContext.ModuleContext);
                using var tempResult = thenContext.IL.TempLocal(noneType);
                tempResult.EmitLoadPtr(context);
                thenContext.IL.Emit(OpCodes.Initobj, noneType, 1, 0);
                tempResult.EmitLoad(thenContext);
            }
        } else {
            var optionType = new OptionType(thenResultType);
            var generatedType = optionType.GeneratedType(thenContext.ModuleContext);
            using var tempResult = thenContext.IL.TempLocal(generatedType);
            var skipThen = thenContext.IL.DefineLabel(thenCount.opCodes < 114);

            thenContext.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
            thenExpression.Prepare(thenContext);
            tempResult.EmitLoadPtr(context);
            thenContext.IL.Emit(OpCodes.Dup);
            thenContext.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            thenContext.IL.Emit(OpCodes.Dup);
            thenContext.IL.Emit(OpCodes.Ldc_I4_1);
            thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            thenExpression.EmitCode(thenContext, false);
            thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            var ifEnd = context.IL.DefineLabel(true);
            thenContext.IL.Emit(OpCodes.Br_S, ifEnd);

            thenContext.IL.MarkLabel(skipThen);

            tempResult.EmitLoadPtr(context);
            thenContext.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);

            thenContext.IL.MarkLabel(ifEnd);

            tempResult.EmitLoad(thenContext);
        }
    }

    public override REPLValueFuture Eval(REPLContext context) {
        if (condition.ResultType(context.replBlockContext) != BuiltinType.Bool)
            throw new REPLException(this, "Condition of if is not a boolean");

        var thenResultType = thenExpression.ResultType(context.replBlockContext);

        return new REPLIfThenFuture(thenResultType, context, condition, thenExpression);
    }

    private class REPLIfThenFuture(TO2Type to2Type, REPLContext context, Expression condition, Expression thenExpression) : REPLValueFuture(new OptionType(to2Type)) {
        private REPLValueFuture? conditionFuture;
        private IREPLValue? conditionResult;
        private REPLValueFuture? thenFuture;

        public override FutureResult<IREPLValue?> PollValue() {
            conditionFuture ??= condition.Eval(context);
            if (conditionResult == null) {
                var result = conditionFuture.PollValue();

                if (!result.IsReady) return new FutureResult<IREPLValue?>();

                conditionResult = result.value;
            }

            if (conditionResult is REPLBool b) {
                if (!b.boolValue) return new FutureResult<IREPLValue?>(new REPLAny(Type, Option.None<object>()));
            } else {
                throw new REPLException(condition, "Condition of if is not a boolean");
            }

            thenFuture ??= thenExpression.Eval(context);

            var thenResult = thenFuture.PollValue();
            if (!thenResult.IsReady) return new FutureResult<IREPLValue?>();

            return new FutureResult<IREPLValue?>(new REPLAny(Type, Option.Some(thenResult.value)));
        }
    }
}

public class IfThenElse(
    Expression condition,
    Expression thenExpression,
    Expression elseExpression,
    Position start = new(),
    Position end = new())
    : Expression(start, end), IVariableContainer {
    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            condition.VariableContainer = value;
            thenExpression.VariableContainer = this;
            elseExpression.VariableContainer = value;
        }
    }

    public override TypeHint? TypeHint {
        set {
            condition.TypeHint = _ => BuiltinType.Bool;
            thenExpression.TypeHint = value;
            elseExpression.TypeHint = value;
        }
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) => condition.GetScopeVariables(context)?.Get(name);

    public override TO2Type ResultType(IBlockContext context) {
        TO2Type thenType = thenExpression.ResultType(context).UnderlyingType(context.ModuleContext);
        TO2Type elseType = elseExpression.ResultType(context).UnderlyingType(context.ModuleContext);

        return !(thenType is OptionType) && elseType is OptionType ? new OptionType(thenType) : thenType;
    }

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (condition.ResultType(context) != BuiltinType.Bool) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    "Condition of if is not a boolean",
                    Start,
                    End
                )
            );
            return;
        }

        var thenContext = context.CreateChildContext();
        var elseContext = context.CreateChildContext();

        var scopeVariables = condition.GetScopeVariables(thenContext);

        if (scopeVariables != null)
            foreach (var (name, type) in scopeVariables.Select(x => (x.Key, x.Value))) {
                if (thenContext.FindVariable(name) != null) {
                    thenContext.AddError(new StructuralError(
                        StructuralError.ErrorType.DuplicateVariableName,
                        $"Variable '{name}' already declared in this scope",
                        Start,
                        End
                    ));
                    return;
                }

                thenContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
            }

        var thenCount = thenExpression.GetILCount(thenContext, dropResult);
        var elseCount = elseExpression.GetILCount(elseContext, dropResult);

        if (!context.HasErrors && thenCount.stack > 1) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    "Then expression leaves too many values on stack. This must not happen",
                    Start,
                    End
                )
            );
            return;
        }

        if (!context.HasErrors && elseCount.stack > 1) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    "Else expression leaves too many values on stack. This must not happen",
                    Start,
                    End
                )
            );
            return;
        }

        condition.EmitCode(thenContext, false);

        if (context.HasErrors) return;

        TO2Type thenType = thenExpression.ResultType(thenContext).UnderlyingType(context.ModuleContext);
        TO2Type elseType = elseExpression.ResultType(elseContext).UnderlyingType(context.ModuleContext);
        var wrapOption = !(thenType is OptionType) && elseType is OptionType;

        if (wrapOption) thenType = new OptionType(thenType);

        if (!dropResult)
            if (!thenType.IsAssignableFrom(context.ModuleContext, elseType))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"If condition has incompatible result {thenType} != {elseType}",
                    Start,
                    End
                ));

        if (context.HasErrors) return;

        var thenEnd =
            context.IL.DefineLabel(!dropResult && wrapOption ? thenCount.opCodes < 114 : thenCount.opCodes < 124);
        var elseEnd = context.IL.DefineLabel(elseCount.opCodes < 124);

        context.IL.Emit(thenEnd.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, thenEnd);
        if (!dropResult && wrapOption) {
            var generatedType = thenType.GeneratedType(thenContext.ModuleContext);
            using var tempResult = thenContext.IL.TempLocal(generatedType);

            thenExpression.Prepare(thenContext);
            tempResult.EmitLoadPtr(context);
            thenContext.IL.Emit(OpCodes.Dup);
            thenContext.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            thenContext.IL.Emit(OpCodes.Dup);
            thenContext.IL.Emit(OpCodes.Ldc_I4_1);
            thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            thenExpression.EmitCode(thenContext, false);
            thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            tempResult.EmitLoad(thenContext);
        } else {
            thenExpression.EmitCode(thenContext, dropResult);
        }

        context.IL.Emit(elseEnd.isShort ? OpCodes.Br_S : OpCodes.Br, elseEnd);
        context.IL.MarkLabel(thenEnd);
        if (!dropResult) context.IL.AdjustStack(-1); // Then leave its result on the stack, so is else supposed to
        elseExpression.EmitCode(elseContext, dropResult);
        if (!dropResult) thenType.AssignFrom(context.ModuleContext, elseType).EmitConvert(context, false);
        context.IL.MarkLabel(elseEnd);
    }

    internal override Expression CollapseFinalReturn() {
        return new IfThenElse(condition, thenExpression.CollapseFinalReturn(), elseExpression.CollapseFinalReturn());
    }

    public override REPLValueFuture Eval(REPLContext context) {
        if (condition.ResultType(context.replBlockContext) != BuiltinType.Bool)
            throw new REPLException(this, "Condition of if is not a boolean");

        var thenResultType = thenExpression.ResultType(context.replBlockContext);

        return new REPLIfThenElseFuture(thenResultType, context, condition, thenExpression, elseExpression);
    }

    private class REPLIfThenElseFuture(
        TO2Type to2Type,
        REPLContext context,
        Expression condition,
        Expression thenExpression,
        Expression elseExpression)
        : REPLValueFuture(new OptionType(to2Type)) {
        private REPLValueFuture? conditionFuture;
        private IREPLValue? conditionResult;
        private REPLValueFuture? elseFuture;
        private REPLValueFuture? thenFuture;

        public override FutureResult<IREPLValue?> PollValue() {
            conditionFuture ??= condition.Eval(context);
            if (conditionResult == null) {
                var result = conditionFuture.PollValue();

                if (!result.IsReady) return new FutureResult<IREPLValue?>();

                conditionResult = result.value;
            }

            if (conditionResult is REPLBool b) {
                if (b.boolValue) {
                    thenFuture ??= thenExpression.Eval(context);

                    var thenResult = thenFuture.PollValue();
                    if (!thenResult.IsReady) return new FutureResult<IREPLValue?>();

                    return new FutureResult<IREPLValue?>(new REPLAny(Type, Option.Some(thenResult.value)));
                }

                elseFuture ??= elseExpression.Eval(context);

                var elseResult = elseFuture.PollValue();
                if (!elseResult.IsReady) return new FutureResult<IREPLValue?>();

                return new FutureResult<IREPLValue?>(new REPLAny(Type, Option.Some(elseResult.value)));
            }

            throw new REPLException(condition, "Condition of if is not a boolean");
        }
    }
}
