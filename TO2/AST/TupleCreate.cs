using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class TupleCreate(List<Expression> items, Position start, Position end) : Expression(start, end) {
    private TupleType? resultType;

    private TypeHint? typeHint;

    public override IVariableContainer? VariableContainer {
        set {
            foreach (var item in items) item.VariableContainer = value;
        }
    }

    public override TypeHint? TypeHint {
        set {
            typeHint = value;
            for (var j = 0; j < items.Count; j++) {
                var i = j; // Copy for lambda
                items[i].TypeHint = context => {
                    var itemTypes = (typeHint?.Invoke(context) as TupleType)?.itemTypes
                        .Select(t => t.UnderlyingType(context.ModuleContext)).ToList();

                    return itemTypes != null && i < itemTypes.Count ? itemTypes[i] : null;
                };
            }
        }
    }

    public override TO2Type ResultType(IBlockContext context) => DeriveType(context);

    public override void Prepare(IBlockContext context) {
        foreach (var item in items) item.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (dropResult) return;

        var tupleHint = ResultType(context) as TupleType;

        using var tempVariable = context.MakeTempVariable(tupleHint ?? DeriveType(context));
        EmitStore(context, tempVariable, false);
    }

    public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
        var tupleType = variable.Type as TupleType;
        if (tupleType == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"{variable.Type} is not a tuple",
                Start,
                End
            ));
            return;
        }

        if (items.Count != tupleType.itemTypes.Count)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected tuple of {tupleType.itemTypes.Count} items, found {items.Count} items",
                Start,
                End
            ));

        for (var i = 0; i < items.Count; i++) {
            var valueType = items[i].ResultType(context);
            if (!tupleType.itemTypes[i].IsAssignableFrom(context.ModuleContext, valueType))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Expected item {i} of {tupleType} to be a {tupleType.itemTypes[i]}, found {valueType}",
                    Start,
                    End
                ));
        }

        if (context.HasErrors) return;

        foreach (var item in items) item.Prepare(context);

        var type = tupleType.GeneratedType(context.ModuleContext);

        variable.EmitLoadPtr(context);
        // Note: Potentially overoptimized: Since all fields will be set, initialization should not be necessary
        //            context.IL.Emit(OpCodes.Dup);
        //            context.IL.Emit(OpCodes.Initobj, type, 1, 0);

        for (var i = 0; i < items.Count; i++) {
            if (i > 0 && i % 7 == 0) {
                context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                type = type.GetGenericArguments()[7];
                //                    context.IL.Emit(OpCodes.Dup);
                //                    context.IL.Emit(OpCodes.Initobj, type, 1, 0);
            }

            if (i < items.Count - 1) context.IL.Emit(OpCodes.Dup);
            items[i].EmitCode(context, false);
            tupleType.itemTypes[i].AssignFrom(context.ModuleContext, items[i].ResultType(context))
                .EmitConvert(context, false);
            context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
        }

        if (context.HasErrors) return;

        if (!dropResult) variable.EmitLoad(context);
    }

    private TupleType DeriveType(IBlockContext context) =>
        resultType ??= new TupleType(items.Select(item => item.ResultType(context)).ToList());

    public override REPLValueFuture Eval(REPLContext context) {
        throw new REPLException(this, "Not supported in REPL mode");
    }

    public override string ToString() => $"({string.Join(", ", items.Select(i => i.ToString()))})";
}
