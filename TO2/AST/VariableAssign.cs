using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class VariableAssign : Expression {
    private readonly Expression expression;
    private readonly string name;
    private readonly Operator op;
    private IVariableContainer? variableContainer;

    public VariableAssign(string name, Operator op, Expression expression, Position start = new(),
        Position end = new()) : base(start, end) {
        this.name = name;
        this.op = op;
        this.expression = expression;
        this.expression.TypeHint = context => ResultType(context).UnderlyingType(context.ModuleContext);
    }

    public override IVariableContainer? VariableContainer {
        set {
            expression.VariableContainer = value;
            variableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) {
        var type = variableContainer?.FindVariable(context, name);

        if (type == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchVariable,
                $"No local variable '{name}'",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        return type;
    }

    public override void Prepare(IBlockContext context) {
        expression.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var blockVariable = context.FindVariable(name);

        if (blockVariable == null)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchVariable,
                $"No local variable '{name}'",
                Start,
                End
            ));
        else if (blockVariable.IsConst)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchVariable,
                $"Local variable '{name}' is read-only (const)",
                Start,
                End
            ));

        if (context.HasErrors) return;

        var valueType = expression.ResultType(context);

        if (context.HasErrors) return;

        if (op == Operator.Assign) {
            EmitAssign(context, blockVariable!, valueType, dropResult);
            return;
        }

        var operatorEmitter = blockVariable!.Type.AllowedSuffixOperators(context.ModuleContext)
            .GetMatching(context.ModuleContext, op, valueType);

        if (operatorEmitter == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Cannot {op} a {blockVariable.Type} with a {valueType}",
                Start,
                End
            ));
            return;
        }

        expression.Prepare(context);

        blockVariable.EmitLoad(context);
        expression.EmitCode(context, false);

        if (context.HasErrors) return;

        operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);
        operatorEmitter.EmitAssign(context, blockVariable, this);

        if (!dropResult) blockVariable.EmitLoad(context);
    }

    private void EmitAssign(IBlockContext context, IBlockVariable blockVariable, TO2Type valueType,
        bool dropResult) {
        if (!blockVariable.Type.IsAssignableFrom(context.ModuleContext, valueType))
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Variable '{name}' is of type {blockVariable.Type} but is assigned to {valueType}",
                Start,
                End
            ));

        if (context.HasErrors) return;

        blockVariable.Type.AssignFrom(context.ModuleContext, valueType)
            .EmitAssign(context, blockVariable, expression, dropResult);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var variable = context.FindVariable(name);

        if (variable == null) throw new REPLException(this, $"No local variable '{name}'");
        if (variable.isConst) throw new REPLException(this, $"Local variable '{name}' is read-only (const)");
        var expressionFuture = expression.Eval(context);
        var assign = variable.declaredType.AssignFrom(context.replModuleContext, expressionFuture.Type);

        if (op == Operator.Assign)
            return expressionFuture.Then(variable.declaredType, value => {
                var converted = assign.EvalConvert(this, value);

                variable.value = converted;

                return converted;
            });
        var operatorEmitter = variable.declaredType.AllowedSuffixOperators(context.replModuleContext)
            .GetMatching(context.replModuleContext, op, expressionFuture.Type);

        if (operatorEmitter == null)
            throw new REPLException(this, $"Cannot {op} a {variable.declaredType} with a {expressionFuture.Type}");

        return expressionFuture.Then(variable.declaredType, value => {
            var converted = assign.EvalConvert(this, operatorEmitter.Eval(this, variable.value!, value));

            variable.value = converted;

            return converted;
        });
    }
}
