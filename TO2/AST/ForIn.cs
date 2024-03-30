using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ForIn : Expression, IVariableContainer {
    private readonly Expression loopExpression;
    private readonly Expression sourceExpression;
    private readonly string variableName;
    private readonly TO2Type? variableType;

    public ForIn(string variableName, TO2Type? variableType, Expression sourceExpression, Expression loopExpression,
        Position start = new(), Position end = new()) : base(start, end) {
        this.variableName = variableName;
        this.variableType = variableType;
        this.sourceExpression = sourceExpression;
        if (this.variableType != null)
            this.sourceExpression.TypeHint = context =>
                new ArrayType(this.variableType.UnderlyingType(context.ModuleContext));
        this.loopExpression = loopExpression;
    }

    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            sourceExpression.VariableContainer = this;
            loopExpression.VariableContainer = this;
        }
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) {
        if (name != variableName) return null;
        return variableType ?? sourceExpression.ResultType(context)?.ForInSource(context.ModuleContext, null)?
            .ElementType;
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var sourceType = sourceExpression.ResultType(context).UnderlyingType(context.ModuleContext);
        var source = sourceType.ForInSource(context.ModuleContext, variableType);

        if (source == null)
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"{sourceType} cannot be use as for ... in source",
                    Start,
                    End
                )
            );
        if (context.FindVariable(variableName) != null)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.DuplicateVariableName,
                $"Variable '{variableName}' already declared in this scope",
                Start,
                End
            ));
        if (source != null && variableType != null &&
            !variableType.IsAssignableFrom(context.ModuleContext, source.ElementType))
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"{sourceType} has elements of type {source.ElementType}, expected {variableType}",
                    Start,
                    End
                )
            );

        if (context.HasErrors) return;

        using var loopCounter = context.IL.TempLocal(typeof(int));
        var loopSize = EstimateLoop(context, source!);
        var start = context.IL.DefineLabel(loopSize.opCodes < 110);
        var end = context.IL.DefineLabel(loopSize.opCodes < 110);
        var loop = context.IL.DefineLabel(loopSize.opCodes < 100);

        var loopContext = context.CreateLoopContext(start, end);
        var loopVariable = loopContext.DeclaredVariable(variableName, true, source!.ElementType);

        sourceExpression.EmitCode(context, false);

        if (context.HasErrors) return;

        source.EmitInitialize(loopContext);
        loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);

        loopContext.IL.MarkLabel(loop);

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

        source.EmitNext(loopContext);
        loopVariable.EmitStore(loopContext);
        loopExpression.EmitCode(loopContext, true);
        loopContext.IL.MarkLabel(start);
        source.EmitCheckDone(loopContext, loop);
        loopContext.IL.MarkLabel(end);
        if (!dropResult) context.IL.Emit(OpCodes.Ldnull);
    }

    private ILCount EstimateLoop(IBlockContext context, IForInSource source) {
        var prepContext = context.CloneCountingContext();

        sourceExpression.EmitCode(prepContext, false);
        source.EmitInitialize(prepContext);

        var countingContext = prepContext.CloneCountingContext()
            .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
        var loopVariable = countingContext.DeclaredVariable(variableName, true, source.ElementType);
        var loop = countingContext.IL.DefineLabel(false);

        source.EmitNext(countingContext);
        loopVariable.EmitStore(countingContext);
        loopExpression.EmitCode(countingContext, true);
        source.EmitCheckDone(countingContext, loop);

        return new ILCount {
            opCodes = countingContext.IL.ILSize,
            stack = countingContext.IL.StackCount
        };
    }

    public override REPLValueFuture Eval(REPLContext context) {
        if (context.FindVariable(variableName) != null)
            throw new REPLException(this, $"Variable '{variableName}' already declared in this scope");

        return new REPLForInFuture(context.CreateChildContext(), variableName, variableType!, sourceExpression,
            loopExpression);
    }

    internal class REPLForInFuture(
        REPLContext context,
        string variableName,
        TO2Type variableType,
        Expression sourceExpression,
        Expression loopExpression)
        : REPLValueFuture(BuiltinType.Unit) {
        private readonly TO2Type? variableType = variableType;
        private IREPLValue? current;
        private REPLValueFuture? loopExpressionFuture;
        private IREPLForInSource? source;
        private REPLValueFuture? sourceFuture;
        private REPLContext.REPLVariable? variable;

        public override FutureResult<IREPLValue?> PollValue() {
            sourceFuture ??= sourceExpression.Eval(context);
            if (source == null) {
                var sourceResult = sourceFuture.PollValue();

                if (!sourceResult.IsReady) return new FutureResult<IREPLValue?>();

                source = sourceResult.value!.ForInSource();

                if (source == null)
                    throw new REPLException(sourceExpression,
                        $"{sourceFuture.Type} cannot be use as for ... in source");

                if (variableType != null) {
                    if (!variableType.IsAssignableFrom(context.replModuleContext, source.ElementType))
                        throw new REPLException(sourceExpression,
                            $"{sourceFuture.Type} has elements of type {source.ElementType}, expected {variableType}");
                    variable = context.DeclaredVariable(variableName, true,
                        variableType.UnderlyingType(context.replModuleContext));
                } else {
                    variable = context.DeclaredVariable(variableName, true,
                        source.ElementType.UnderlyingType(context.replModuleContext));
                }
            }

            if (current == null) {
                current = source.Next();

                if (current == null) return new FutureResult<IREPLValue?>(REPLUnit.INSTANCE);

                variable!.value = variable.declaredType.REPLCast(current.Value);
            }

            loopExpressionFuture ??= loopExpression.Eval(context);
            var loopExpressionResult = loopExpressionFuture.PollValue();

            if (!loopExpressionResult.IsReady) return new FutureResult<IREPLValue?>();

            if (loopExpressionResult.value!.IsBreak) return new FutureResult<IREPLValue?>(REPLUnit.INSTANCE);
            if (loopExpressionResult.value.IsReturn) return new FutureResult<IREPLValue?>(loopExpressionResult.value);
            loopExpressionFuture = null;
            current = null;

            return new FutureResult<IREPLValue?>();
        }
    }
}
