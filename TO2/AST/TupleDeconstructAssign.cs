using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class TupleDeconstructAssign : Expression {
    private readonly Expression expression;
    private readonly List<(string target, string source)> targets;

    public TupleDeconstructAssign(List<(string source, string target)> targets, Expression expression,
        Position start = new(), Position end = new()) : base(start, end) {
        this.targets = targets;
        this.expression = expression;
        this.expression.TypeHint = context => ResultType(context).UnderlyingType(context.ModuleContext);
    }

    public override IVariableContainer? VariableContainer {
        set => expression.VariableContainer = value;
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var valueType = expression.ResultType(context).UnderlyingType(context.ModuleContext);

        switch (valueType) {
        case TupleType tupleType:
            EmitCodeTuple(context, tupleType);
            return;
        case RecordType recordType:
            EmitCodeRecord(context, recordType);
            return;
        default:
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected right side to be a tuple or record, but got {valueType}",
                Start,
                End
            ));
            return;
        }
    }

    private void EmitCodeTuple(IBlockContext context, TupleType tupleType) {
        var variables = new List<(int index, IBlockVariable variable)>();

        for (var i = 0; i < targets.Count; i++) {
            if (targets[i].target.Length == 0) continue;

            var blockVariable = context.FindVariable(targets[i].target);

            if (blockVariable == null)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchVariable,
                    $"No local variable '{targets[i].target}'",
                    Start,
                    End
                ));
            else if (blockVariable.IsConst)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchVariable,
                    $"Local variable '{targets[i].target}' is read-only (const)",
                    Start,
                    End
                ));
            else
                variables.Add((i, blockVariable));
        }

        if (tupleType.itemTypes.Count != targets.Count)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected right side to be a tuple with {targets.Count} elements, but got {tupleType}",
                Start,
                End
            ));

        if (context.HasErrors) return;

        expression.EmitCode(context, false);

        if (context.HasErrors) return;

        foreach (var (index, variable) in variables) {
            var itemAccess = tupleType.FindField(context.ModuleContext, $"_{index + 1}")!
                .Create(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            itemAccess.EmitLoad(context);

            variable.Type.AssignFrom(context.ModuleContext, itemAccess.FieldType).EmitConvert(context, true);
            variable.EmitStore(context);
        }

        context.IL.Emit(OpCodes.Pop);
    }

    private void EmitCodeRecord(IBlockContext context, RecordType recordType) {
        var variables =
            new List<(string field, IBlockVariable variable)>();

        for (var i = 0; i < targets.Count; i++) {
            if (targets[i].target.Length == 0) continue;

            if (!recordType.ItemTypes.ContainsKey(targets[i].source))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"{recordType} does not have a field '{targets[i].source}'",
                    Start,
                    End
                ));

            var blockVariable = context.FindVariable(targets[i].target);

            if (blockVariable == null)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchVariable,
                    $"No local variable '{targets[i].target}'",
                    Start,
                    End
                ));
            else if (blockVariable.IsConst)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchVariable,
                    $"Local variable '{targets[i].target}' is read-only (const)",
                    Start,
                    End
                ));
            else
                variables.Add((targets[i].source, blockVariable));
        }

        if (context.HasErrors) return;

        expression.EmitCode(context, false);

        if (context.HasErrors) return;

        foreach (var (field, variable) in variables) {
            var itemAccess =
                recordType.FindField(context.ModuleContext, field)!.Create(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            itemAccess.EmitLoad(context);

            variable.Type.AssignFrom(context.ModuleContext, itemAccess.FieldType).EmitConvert(context, true);
            variable.EmitStore(context);
        }

        context.IL.Emit(OpCodes.Pop);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        throw new REPLException(this, "Not supported in REPL mode");
    }
}
