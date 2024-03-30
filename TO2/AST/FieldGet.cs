using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class FieldGet(
    Expression target,
    string fieldName,
    Position start = new(),
    Position end = new())
    : Expression(start, end), IAssignContext {
    public override IVariableContainer? VariableContainer {
        set => target.VariableContainer = value;
    }

    public bool IsConst(IBlockContext context) => (target as IAssignContext)?.IsConst(context) ?? true;

    public override TO2Type ResultType(IBlockContext context) {
        var targetType = target.ResultType(context);
        var fieldAccess =
            targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
        if (fieldAccess == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        return fieldAccess.FieldType;
    }

    public override void Prepare(IBlockContext context) {
        target.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var targetType = target.ResultType(context);
        var fieldAccess =
            targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);

        if (fieldAccess == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                Start,
                End
            ));
            return;
        }

        if (!dropResult) {
            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            fieldAccess.EmitLoad(context);
        }
    }

    public override void EmitPtr(IBlockContext context) {
        var targetType = target.ResultType(context);
        var fieldAccess =
            targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);

        if (fieldAccess == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                Start,
                End
            ));
            return;
        }

        if (fieldAccess.RequiresPtr) target.EmitPtr(context);
        else target.EmitCode(context, false);

        if (context.HasErrors) return;

        fieldAccess.EmitPtr(context);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var targetFuture = target.Eval(context);
        var fieldAccess =
            targetFuture.Type.FindField(context.replModuleContext, fieldName)?.Create(context.replModuleContext);

        if (fieldAccess == null)
            throw new REPLException(this, $"Type '{targetFuture.Type.Name}' does not have a field '{fieldName}'");

        return targetFuture.Then(fieldAccess.FieldType, target => fieldAccess.EvalGet(this, target));
    }

    public override string ToString() => $"{target}.{fieldName}";
}
