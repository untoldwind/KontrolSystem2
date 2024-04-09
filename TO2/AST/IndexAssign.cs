using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class IndexAssign(
    Expression target,
    IndexSpec indexSpec,
    Operator op,
    Expression expression,
    Position start = new(),
    Position end = new())
    : Expression(start, end) {
    public override IVariableContainer? VariableContainer {
        set {
            target.VariableContainer = value;
            indexSpec.VariableContainer = value;
            expression.VariableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) {
        return target.ResultType(context)?.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ??
               BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        target.Prepare(context);
        indexSpec.start.Prepare(context);
        expression.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var valueType = expression.ResultType(context);
        var targetType = target.ResultType(context);
        var indexAccess = targetType.AllowedIndexAccess(context.ModuleContext, indexSpec);

        if (indexAccess == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoIndexAccess,
                $"Type '{targetType.Name}' does not support access by index",
                Start,
                End
            ));
            return;
        }

        if (target is IAssignContext assignContext) {
            if (assignContext.IsConst(context)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' elements can not be set on a read-only variable",
                    Start,
                    End
                ));
                return;
            }
        } else {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.CoreGeneration,
                $"Index assign '{targetType.Name}'.'{indexSpec}' on invalid target expression",
                Start,
                End
            ));
            return;
        }

        if (op == Operator.Assign) {
            expression.Prepare(context);

            if (indexAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            if (!dropResult) {
                using var tmpResult =
                    context.MakeTempVariable(indexAccess.TargetType.UnderlyingType(context.ModuleContext));
                indexAccess.EmitStore(context, subContext => {
                    expression.EmitCode(subContext, false);
                    indexAccess.TargetType.AssignFrom(subContext.ModuleContext, valueType)
                        .EmitConvert(subContext, true);

                    context.IL.Emit(OpCodes.Dup);
                    // ReSharper disable once AccessToDisposedClosure
                    tmpResult.EmitStore(subContext);
                });

                tmpResult.EmitLoad(context);
            } else {
                indexAccess.EmitStore(context, subContext => {
                    expression.EmitCode(subContext, false);
                    indexAccess.TargetType.AssignFrom(subContext.ModuleContext, valueType).EmitConvert(subContext, true);
                });
            }
        } else {
            var operatorEmitter = indexAccess.TargetType.AllowedSuffixOperators(context.ModuleContext)
                .GetMatching(context.ModuleContext, op, valueType);

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Index assign '{targetType.Name}'.'{indexSpec}': Cannot {op} a {indexAccess.TargetType} with a {valueType}",
                    Start,
                    End
                ));
                return;
            }

            expression.Prepare(context);

            if (indexAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            if (!dropResult) {
                using var tmpResult =
                    context.MakeTempVariable(indexAccess.TargetType.UnderlyingType(context.ModuleContext));
                indexAccess.EmitStore(context, subContext => {
                    if (indexAccess.RequiresPtr) target.EmitPtr(context);
                    else target.EmitCode(context, false);

                    indexAccess.EmitLoad(context);
                    expression.EmitCode(subContext, false);

                    operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context, true);
                    operatorEmitter.EmitCode(context, this);

                    context.IL.Emit(OpCodes.Dup);
                    // ReSharper disable once AccessToDisposedClosure
                    tmpResult.EmitStore(subContext);
                });

                tmpResult.EmitLoad(context);
            } else {
                indexAccess.EmitStore(context, subContext => {
                    if (indexAccess.RequiresPtr) target.EmitPtr(context);
                    else target.EmitCode(context, false);

                    indexAccess.EmitLoad(context);
                    expression.EmitCode(subContext, false);

                    operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context, true);
                    operatorEmitter.EmitCode(context, this);
                });
            }
        }
    }
}
