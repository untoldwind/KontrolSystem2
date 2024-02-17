using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType : RealizedType {
    public static readonly OperatorCollection NoOperators = new();

    public static readonly Dictionary<string, IMethodInvokeFactory> NoMethods = new();

    public static readonly Dictionary<string, IFieldAccessFactory> NoFields = new();

    public static readonly RealizedType Unit = new TO2Unit();
    public static readonly RealizedType Bool = new TO2Bool();
    public static readonly RealizedType Int = new TO2Int();
    public static readonly RealizedType Float = new TO2Float();
    public static readonly RealizedType String = new TO2SString();
    public static readonly RealizedType Range = new RangeType();

    public static readonly RealizedType ArrayBuilder = new BoundType(null, "ArrayBuilder",
        "Helper to create an array of initially unknown size", typeof(ArrayBuilder<>),
        NoOperators,
        new OperatorCollection {
            {
                Operator.AddAssign,
                new StaticMethodOperatorEmitter(() => new GenericParameter("T"), () => ArrayBuilder!,
                    typeof(ArrayBuilderOps).GetMethod("AddTo"))
            }
        },
        new List<(string name, IMethodInvokeFactory invoker)> {
            ("append",
                new BoundMethodInvokeFactory("Append an element to the array", true, () => ArrayBuilder!,
                    () => new List<RealizedParameter> {
                        new("element", new GenericParameter("T"), "Value ot append")
                    },
                    false, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Append"))),
            ("result",
                new BoundMethodInvokeFactory("Build the resulting array", true,
                    () => new ArrayType(new GenericParameter("T")), () => new List<RealizedParameter>(), false,
                    typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Result")))
        },
        new List<(string name, IFieldAccessFactory access)> {
            ("length",
                new BoundPropertyLikeFieldAccessFactory("", () => Int, typeof(ArrayBuilder<>),
                    typeof(ArrayBuilder<>).GetProperty("Length")))
        }
    );

    public static readonly RealizedType Cell = new BoundType(null, "Cell",
        "Holds a single value that can be mutated at any time", typeof(Cell<>),
        NoOperators,
        NoOperators,
        new List<(string name, IMethodInvokeFactory invoker)> {
            ("set_value",
                new BoundMethodInvokeFactory("Set the value of the cell", true, () => Unit,
                    () => new List<RealizedParameter> {
                        new("value", new GenericParameter("T"), "New value of the cell")
                    },
                    false, typeof(Cell<>), typeof(Cell<>).GetProperty("Value")?.SetMethod)),
            ("update",
                new BoundMethodInvokeFactory("Atomically update the value of the cell", true,
                    () => Cell!,
                    () => new List<RealizedParameter> {
                        new("updater",
                            new FunctionType(false, new List<TO2Type> { new GenericParameter("T") },
                                new GenericParameter("T")),
                            "Function to be applied on current value of the cell returning the new value of the cell")
                    }, false, typeof(Cell<>), typeof(Cell<>).GetMethod("Update")))
        },
        new List<(string name, IFieldAccessFactory access)> {
            ("value",
                new BoundPropertyLikeFieldAccessFactory("", () => new GenericParameter("T"), typeof(Cell<>),
                    typeof(Cell<>).GetProperty("Value")))
        }
    );

    public override RealizedType UnderlyingType(ModuleContext context) {
        return this;
    }

    public static TO2Type? GetBuiltinType(List<string> namePath, List<TO2Type> typeArguments) {
        if (namePath.Count != 1) return null;

        switch (namePath[0]) {
        case "Unit" when typeArguments.Count == 0: return Unit;
        case "bool" when typeArguments.Count == 0: return Bool;
        case "int" when typeArguments.Count == 0: return Int;
        case "float" when typeArguments.Count == 0: return Float;
        case "string" when typeArguments.Count == 0: return String;
        case "Range" when typeArguments.Count == 0: return Range;
        case "Option" when typeArguments.Count == 1: return new OptionType(typeArguments[0]);
        case "Result" when typeArguments.Count == 2: return new ResultType(typeArguments[0], typeArguments[1]);
        default: return null;
        }
    }
}
