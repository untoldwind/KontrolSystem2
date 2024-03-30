using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class While : Expression, IVariableContainer {
    private readonly Expression condition;
    private readonly Expression loopExpression;

    public While(Expression condition, Expression loopExpression, Position start = new(),
        Position end = new()) : base(start, end) {
        this.condition = condition;
        this.condition.TypeHint = _ => BuiltinType.Bool;
        this.loopExpression = loopExpression;
    }

    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            condition.VariableContainer = value;
            loopExpression.VariableContainer = this;
        }
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) => condition.GetScopeVariables(context)?.Get(name);

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (condition.ResultType(context) != BuiltinType.Bool)
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    "Condition of while is not a boolean",
                    Start,
                    End
                )
            );

        var tmpContext =
            context.CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
        var scopeVariables = condition.GetScopeVariables(tmpContext);

        if (scopeVariables != null)
            foreach (var (name, type) in scopeVariables.Select(x => (x.Key, x.Value)))
                tmpContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));

        var conditionCount = condition.GetILCount(tmpContext, false);
        var loopCount = loopExpression.GetILCount(tmpContext, true);

        if (loopCount.stack > 0) {
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    "Body of the while expression leaves values on stack. This must not happen",
                    Start,
                    End
                )
            );
            return;
        }

        if (context.HasErrors) return;

        using var loopCounter = context.IL.TempLocal(typeof(int));
        var whileStart = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 110);
        var whileEnd = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 110);
        var whileLoop = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 100);
        var loopContext = context.CreateLoopContext(whileStart, whileEnd);

        if (scopeVariables != null)
            foreach (var (name, type) in scopeVariables.Select(x => (x.Key, x.Value))) {
                if (loopContext.FindVariable(name) != null) {
                    loopContext.AddError(new StructuralError(
                        StructuralError.ErrorType.DuplicateVariableName,
                        $"Variable '{name}' already declared in this scope",
                        Start,
                        End
                    ));
                    return;
                }

                loopContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
            }

        loopContext.IL.Emit(whileStart.isShort ? OpCodes.Br_S : OpCodes.Br, whileStart);
        context.IL.MarkLabel(whileLoop);

        // Timeout check
        var skipCheck = context.IL.DefineLabel(true);
        loopCounter.EmitLoad(loopContext);
        loopContext.IL.Emit(OpCodes.Ldc_I4_1);
        loopContext.IL.Emit(OpCodes.Add);
        loopContext.IL.Emit(OpCodes.Dup);
        loopCounter.EmitStore(loopContext);
        loopContext.IL.Emit(OpCodes.Ldc_I4, 10000);
        loopContext.IL.Emit(OpCodes.Cgt);
        loopContext.IL.Emit(OpCodes.Brfalse, skipCheck);
        loopContext.IL.Emit(OpCodes.Ldc_I4_0);
        loopCounter.EmitStore(loopContext);
        ILChunks.GenerateCheckTimeout(context);
        loopContext.IL.MarkLabel(skipCheck);

        loopExpression.EmitCode(loopContext, true);
        loopContext.IL.MarkLabel(whileStart);
        condition.EmitCode(loopContext, false);

        loopContext.IL.Emit(whileLoop.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, whileLoop);

        loopContext.IL.MarkLabel(whileEnd);
        if (!dropResult) context.IL.Emit(OpCodes.Ldnull);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        if (condition.ResultType(context.replBlockContext) != BuiltinType.Bool)
            throw new REPLException(this, "Condition of while is not a boolean");
        return new REPLWhileFuture(context.CreateChildContext(), condition, loopExpression);
    }

    internal class REPLWhileFuture : REPLValueFuture {
        private readonly Expression condition;
        private readonly REPLContext context;
        private readonly Expression loopExpression;
        private REPLValueFuture? conditionFuture;
        private IREPLValue? conditionResult;
        private REPLValueFuture? loopExpressionFuture;

        public REPLWhileFuture(REPLContext context, Expression condition, Expression loopExpression) : base(BuiltinType
            .Unit) {
            this.context = context;
            this.condition = condition;
            this.loopExpression = loopExpression;
        }

        public override FutureResult<IREPLValue?> PollValue() {
            conditionFuture ??= condition.Eval(context);

            if (conditionResult == null) {
                var result = conditionFuture.PollValue();

                if (!result.IsReady) return new FutureResult<IREPLValue?>();

                conditionResult = result.value;
            }

            if (conditionResult is REPLBool b) {
                if (!b.boolValue) return new FutureResult<IREPLValue?>(REPLUnit.INSTANCE);
            } else {
                throw new REPLException(condition, "Condition of while is not a boolean");
            }

            loopExpressionFuture ??= loopExpression.Eval(context);
            var loopExpressionResult = loopExpressionFuture.PollValue();

            if (!loopExpressionResult.IsReady) return new FutureResult<IREPLValue?>();

            if (loopExpressionResult.value!.IsBreak) return new FutureResult<IREPLValue?>(REPLUnit.INSTANCE);
            if (loopExpressionResult.value.IsReturn) return new FutureResult<IREPLValue?>(loopExpressionResult.value);
            loopExpressionFuture = null;
            conditionFuture = null;
            conditionResult = null;

            return new FutureResult<IREPLValue?>();
        }
    }
}
