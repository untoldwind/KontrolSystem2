using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class FieldAssign : Expression {
    private readonly Expression target;
    private readonly string fieldName;
    private readonly Operator op;
    private readonly Expression expression;

    public FieldAssign(Expression target, string fieldName, Operator op, Expression expression,
        Position start = new(), Position end = new())
        : base(start, end) {
        this.target = target;
        this.fieldName = fieldName;
        this.op = op;
        this.expression = expression;
        this.expression.TypeHint = context => ResultType(context).UnderlyingType(context.ModuleContext);
    }

    public override IVariableContainer? VariableContainer {
        set {
            target.VariableContainer = value;
            expression.VariableContainer = value;
        }
    }

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

        if (!fieldAccess.CanStore) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        return fieldAccess.FieldType;
    }

    public override void Prepare(IBlockContext context) {
        target.Prepare(context);
        expression.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var targetType = target.ResultType(context);
        var fieldAccess =
            targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
        var valueType = expression.ResultType(context);

        if (fieldAccess == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                Start,
                End
            ));
            return;
        }

        if (!fieldAccess.CanStore) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchField,
                $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                Start,
                End
            ));
            return;
        }

        if (fieldAccess.IsAsyncStore && !context.IsAsync) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchFunction,
                $"Type '{targetType.Name}' field '{fieldName}' can not be changed in a sync context",
                Start,
                End
            ));
            return;
        }

        if (target is IAssignContext assignContext) {
            if (fieldAccess.RequiresPtr && assignContext.IsConst(context)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' field '{fieldName}' can not be set on a read-only variable",
                    Start,
                    End
                ));
                return;
            }
        } else {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.CoreGeneration,
                $"Field assign '{targetType.Name}'.'{fieldName}' on invalid target expression",
                Start,
                End
            ));
            return;
        }

        if (op == Operator.Assign) {
            if (!fieldAccess.FieldType.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Type '{targetType.Name}' field '{fieldName}' is of type {fieldAccess.FieldType} but is assigned to {valueType}",
                    Start,
                    End
                ));
                return;
            }

            expression.Prepare(context);

            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            expression.EmitCode(context, false);
            fieldAccess.FieldType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context, true);

            if (!dropResult) {
                using var tmpResult = context.MakeTempVariable(fieldAccess.FieldType);

                context.IL.Emit(OpCodes.Dup);
                tmpResult.EmitStore(context);

                fieldAccess.EmitStore(context);

                if (fieldAccess.IsAsyncStore) {
                    context.IL.Emit(OpCodes.Ldnull);
                    context.IL.EmitNew(OpCodes.Newobj,
                        typeof(Future.Success<object>).GetConstructor([typeof(object)])!);
                    context.RegisterAsyncResume(BuiltinType.Unit);
                    context.IL.Emit(OpCodes.Pop);
                }

                tmpResult.EmitLoad(context);
            } else {
                fieldAccess.EmitStore(context);
                if (fieldAccess.IsAsyncStore) {
                    context.IL.Emit(OpCodes.Ldnull);
                    context.IL.EmitNew(OpCodes.Newobj,
                        typeof(Future.Success<object>).GetConstructor([typeof(object)])!);
                    context.RegisterAsyncResume(BuiltinType.Unit);
                    context.IL.Emit(OpCodes.Pop);
                }
            }
        } else {
            var operatorEmitter = fieldAccess.FieldType.AllowedSuffixOperators(context.ModuleContext)
                .GetMatching(context.ModuleContext, op, valueType);

            if (operatorEmitter == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Type '{targetType.Name}' field '{fieldName}': Cannot {op} a {fieldAccess.FieldType} with a {valueType}",
                    Start,
                    End
                ));
                return;
            }

            expression.Prepare(context);

            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            fieldAccess.EmitLoad(context);
            expression.EmitCode(context, false);
            operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context, true);
            operatorEmitter.EmitCode(context, this);

            if (!dropResult) {
                using var tmpResult = context.MakeTempVariable(fieldAccess.FieldType);

                context.IL.Emit(OpCodes.Dup);
                tmpResult.EmitStore(context);

                fieldAccess.EmitStore(context);

                if (fieldAccess.IsAsyncStore) {
                    context.IL.Emit(OpCodes.Ldnull);
                    context.IL.EmitNew(OpCodes.Newobj,
                        typeof(Future.Success<object>).GetConstructor([typeof(object)])!);
                    context.RegisterAsyncResume(BuiltinType.Unit);
                    context.IL.Emit(OpCodes.Pop);
                }

                tmpResult.EmitLoad(context);
            } else {
                fieldAccess.EmitStore(context);

                if (fieldAccess.IsAsyncStore) {
                    context.IL.Emit(OpCodes.Ldnull);
                    context.IL.EmitNew(OpCodes.Newobj,
                        typeof(Future.Success<object>).GetConstructor([typeof(object)])!);
                    context.RegisterAsyncResume(BuiltinType.Unit);
                    context.IL.Emit(OpCodes.Pop);
                }
            }
        }
    }
}
