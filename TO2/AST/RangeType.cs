using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;
using Range = KontrolSystem.TO2.Runtime.Range;

namespace KontrolSystem.TO2.AST;

public class RangeType : RealizedType {
    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; } = new() {
        {
            "map", new BoundMethodInvokeFactory("Map the elements of the range, i.e. convert it into an array.",
                true,
                () => new ArrayType(new GenericParameter("T")),
                () => [
                    new("mapper",
                        new FunctionType(false, [BuiltinType.Int], new GenericParameter("T")),
                        "Function to be applied on each element of the range")
                ],
                false, typeof(Range), typeof(Range).GetMethod("Map"))
        }, {
            "flat_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("T")),
                () => [
                    new("mapper", new FunctionType(false, [BuiltinType.Int], new ArrayType(new GenericParameter("T"))),
                        "Function to be applied on each element of the range")
                ],
                false, typeof(Range), typeof(Range).GetMethod("FlatMap"))
        }, {
            "filter_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("T")),
                () => [
                    new("mapper", new FunctionType(false, [BuiltinType.Int], new OptionType(new GenericParameter("T"))),
                        "Function to be applied on each element of the range")
                ],
                false, typeof(Range), typeof(Range).GetMethod("FilterMap"))
        }, {
            "reduce",
            new BoundMethodInvokeFactory("Reduce range by an operation", true, () => new GenericParameter("U"),
                () => [
                    new("initial", new GenericParameter("U"), "Initial value of the accumulator"),
                    new("reducer", new FunctionType(false, [
                            new GenericParameter("U"),
                            BuiltinType.Int
                        ], new GenericParameter("U")),
                        "Combines accumulator with each element of the array and returns new accumulator value")
                ], false, typeof(Range),
                typeof(Range).GetMethod("Reduce"))
        }, {
            "reverse", new BoundMethodInvokeFactory("Reverse order", true,
                () => new ArrayType(BuiltinType.Int),
                () => [],
                false, typeof(Range), typeof(Range).GetMethod("Reverse"))
        }, {
            "to_string", new BoundMethodInvokeFactory("Get string representation of the range", true,
                () => BuiltinType.String,
                () => [],
                false, typeof(Range), typeof(Range).GetMethod("RangeToString"))
        }
    };

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; } = new() {
        {
            "length",
            new BoundPropertyLikeFieldAccessFactory("The length of the range", () => BuiltinType.Int,
                typeof(Range), typeof(Range).GetProperty("Length"))
        }
    };

    public override string Name => "Range";

    public override bool IsValid(ModuleContext context) => true;

    public override RealizedType UnderlyingType(ModuleContext context) => this;

    public override Type GeneratedType(ModuleContext context) => typeof(Range);

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

    public override IForInSource ForInSource(ModuleContext context, TO2Type? typeHint) => new RangeForInSource();

    public override IREPLValue REPLCast(object? value) => new REPLRange((Range)value!);
}

public class RangeForInSource : IForInSource {
    private ILocalRef? currentIndex;
    private ILocalRef? rangeRef;

    public RealizedType ElementType => BuiltinType.Int;

    public void EmitInitialize(IBlockContext context) {
        rangeRef = context.DeclareHiddenLocal(typeof(Range));
        rangeRef.EmitStore(context);
        currentIndex = context.DeclareHiddenLocal(typeof(long));
        rangeRef.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("from"));
        context.IL.Emit(OpCodes.Ldc_I4_1);
        context.IL.Emit(OpCodes.Conv_I8);
        context.IL.Emit(OpCodes.Sub);
        currentIndex.EmitStore(context);
    }

    public void EmitCheckDone(IBlockContext context, LabelRef loop) {
        currentIndex!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldc_I4_1);
        context.IL.Emit(OpCodes.Conv_I8);
        context.IL.Emit(OpCodes.Add);
        context.IL.Emit(OpCodes.Dup);
        currentIndex!.EmitStore(context);
        rangeRef!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("to"));
        context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
    }

    public void EmitNext(IBlockContext context) {
        currentIndex!.EmitLoad(context);
    }
}
