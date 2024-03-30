using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class RangeCreate : Expression {
    private readonly Expression from;
    private readonly bool inclusive;
    private readonly Expression to;

    public RangeCreate(Expression from, Expression to, bool inclusive, Position start = new(),
        Position end = new()) : base(start, end) {
        this.from = from;
        this.to = to;
        this.inclusive = inclusive;

        from.TypeHint = _ => BuiltinType.Int;
        this.to.TypeHint = _ => BuiltinType.Int;
    }

    public override IVariableContainer? VariableContainer {
        set {
            from.VariableContainer = value;
            to.VariableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Range;

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (dropResult) return;

        using var tempVariable = context.MakeTempVariable(BuiltinType.Range);
        EmitStore(context, tempVariable, false);
    }

    public override void Prepare(IBlockContext context) {
        from.Prepare(context);
        to.Prepare(context);
    }

    public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
        if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, from.ResultType(context)))
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                "Range can only be created from int values",
                from.Start,
                from.End
            ));
        if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, to.ResultType(context)))
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                "Range can only be created from int values",
                to.Start,
                to.End
            ));

        if (context.HasErrors) return;

        variable.EmitLoadPtr(context);
        context.IL.Emit(OpCodes.Dup);
        from.EmitCode(context, false);
        context.IL.Emit(OpCodes.Stfld, typeof(Range).GetField("from"));
        to.EmitCode(context, false);
        if (inclusive) {
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Conv_I8);
            context.IL.Emit(OpCodes.Add);
        }

        context.IL.Emit(OpCodes.Stfld, typeof(Range).GetField("to"));

        if (!dropResult) variable.EmitLoad(context);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var fromFuture = from.Eval(context);
        var toFuture = to.Eval(context);

        if (!BuiltinType.Int.IsAssignableFrom(context.replModuleContext, fromFuture.Type) ||
            !BuiltinType.Int.IsAssignableFrom(context.replModuleContext, toFuture.Type))
            throw new REPLException(this, "Range can only be created from int values");

        return REPLValueFuture.Chain2(BuiltinType.Range, fromFuture, toFuture, (from, to) => {
            if (from is REPLInt ifrom && to is REPLInt ito)
                return new REPLRange(new Range(ifrom.intValue, inclusive ? ito.intValue + 1 : ito.intValue));

            throw new REPLException(this, "Can create range from non-integers");
        });
    }

    public override string ToString() => inclusive ? $"{from}...{to}" : $"{from}..{to}";
}
