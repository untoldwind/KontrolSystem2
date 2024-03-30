using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType : RealizedType {
    public static readonly OperatorCollection NoOperators = [];

    public static readonly Dictionary<string, IMethodInvokeFactory> NoMethods = [];

    public static readonly Dictionary<string, IFieldAccessFactory> NoFields = [];

    public static readonly RealizedType Unit = new TO2Unit();
    public static readonly RealizedType Bool = new TO2Bool();
    public static readonly RealizedType Int = new TO2Int();
    public static readonly RealizedType Float = new TO2Float();
    public static readonly RealizedType String = new TO2SString();
    public static readonly RealizedType Range = new RangeType();

    public static readonly TO2Type Error =
        new LookupTypeReference(["core", "error", "Error"], [], new Position(), new Position());

    public static readonly RealizedType ArrayBuilder = new BoundType(null, "ArrayBuilder",
        "Helper to create an array of initially unknown size", typeof(ArrayBuilder<>),
        NoOperators,
        new OperatorCollection {
            {
                Operator.AddAssign,
                new StaticMethodOperatorEmitter(() => new GenericParameter("T"), LazyArrayBuilder,
                    typeof(ArrayBuilderOps).GetMethod("AddTo"))
            }
        },
        [
            ("append",
                new BoundMethodInvokeFactory("Append an element to the array", true, LazyArrayBuilder,
                    () => [new("element", new GenericParameter("T"), "Value ot append")],
                    false, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Append"))),
            ("result",
                new BoundMethodInvokeFactory("Build the resulting array", true,
                    () => new ArrayType(new GenericParameter("T")), () => [], false,
                    typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Result")))
        ],
        [
            ("length",
                new BoundPropertyLikeFieldAccessFactory("", () => Int, typeof(ArrayBuilder<>),
                    typeof(ArrayBuilder<>).GetProperty("Length")))
        ]
    );

    private static RealizedType LazyArrayBuilder() => ArrayBuilder;

    public static readonly RealizedType Cell = new BoundType(null, "Cell",
        "Holds a single value that can be mutated at any time", typeof(Cell<>),
        NoOperators,
        NoOperators,
        [
            ("set_value",
                new BoundMethodInvokeFactory("Set the value of the cell", true, () => Unit,
                    () => [new("value", new GenericParameter("T"), "New value of the cell")],
                    false, typeof(Cell<>), typeof(Cell<>).GetProperty("Value")?.SetMethod)),
            ("update",
                new BoundMethodInvokeFactory("Atomically update the value of the cell", true,
                    LazyCell,
                    () => [
                        new("updater",
                            new FunctionType(false, [new GenericParameter("T")],
                                new GenericParameter("T")),
                            "Function to be applied on current value of the cell returning the new value of the cell")
                    ], false, typeof(Cell<>), typeof(Cell<>).GetMethod("Update")))
        ],
        [
            ("value",
                new BoundPropertyLikeFieldAccessFactory("", () => new GenericParameter("T"), typeof(Cell<>),
                    typeof(Cell<>).GetProperty("Value")))
        ]
    );

    private static RealizedType LazyCell() => Cell;

    public override RealizedType UnderlyingType(ModuleContext context) => this;

    public static TO2Type? GetBuiltinType(List<string> namePath, List<TO2Type> typeArguments) {
        if (namePath.Count != 1) return null;

        return namePath[0] switch {
            "Unit" when typeArguments.Count == 0 => Unit,
            "bool" when typeArguments.Count == 0 => Bool,
            "int" when typeArguments.Count == 0 => Int,
            "float" when typeArguments.Count == 0 => Float,
            "string" when typeArguments.Count == 0 => String,
            "Range" when typeArguments.Count == 0 => Range,
            "Option" when typeArguments.Count == 1 => new OptionType(typeArguments[0]),
            "Result" when typeArguments.Count == 1 => new ResultType(typeArguments[0]),
            // Deprecated Result type
            "Result" when typeArguments.Count == 2 => new ResultType(typeArguments[0]),
            _ => null,
        };
    }
}
