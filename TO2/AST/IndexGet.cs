using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class IndexGet(
    Expression target,
    IndexSpec indexSpec,
    Position start = new(),
    Position end = new())
    : Expression(start, end), IAssignContext {
    public override IVariableContainer? VariableContainer {
        set {
            target.VariableContainer = value;
            indexSpec.VariableContainer = value;
        }
    }

    public bool IsConst(IBlockContext context) {
        return (target as IAssignContext)?.IsConst(context) ?? true;
    }

    public override TO2Type ResultType(IBlockContext context) {
        var targetType = target.ResultType(context);
        if (targetType is BoundValueType bound)
            targetType = bound.elementType;

        return targetType.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ?? BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        target.Prepare(context);
        indexSpec.start.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var targetType = target.ResultType(context);
        IAssignEmitter? targetConvert = null;

        if (targetType is BoundValueType bound) {
            targetType = bound.elementType;
            targetConvert = bound.elementType.AssignFrom(context.ModuleContext, bound);
        }

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

        if (!dropResult) {
            if (indexAccess.RequiresPtr) {
                if (targetConvert != null) {
                    target.EmitCode(context, false);
                    targetConvert.EmitConvert(context, false);
                    using var tempLocal =
                        context.MakeTempVariable(targetType.UnderlyingType(context.ModuleContext));
                    tempLocal.EmitStore(context);
                    tempLocal.EmitLoadPtr(context);
                } else
                    target.EmitPtr(context);
            } else {
                target.EmitCode(context, false);
                targetConvert?.EmitConvert(context, false);
            }

            if (context.HasErrors) return;

            indexAccess.EmitLoad(context);
        }
    }

    public override void EmitPtr(IBlockContext context) {
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

        if (indexAccess.RequiresPtr) target.EmitPtr(context);
        else target.EmitCode(context, false);

        if (context.HasErrors) return;

        indexAccess.EmitPtr(context);
    }

    public override string ToString() => $"{target}[{indexSpec}]";
}
