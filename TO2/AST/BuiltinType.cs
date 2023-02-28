using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType {
        public static readonly OperatorCollection NoOperators = new OperatorCollection();

        public static readonly Dictionary<string, IMethodInvokeFactory> NoMethods =
            new Dictionary<string, IMethodInvokeFactory>();

        public static readonly Dictionary<string, IFieldAccessFactory> NoFields =
            new Dictionary<string, IFieldAccessFactory>();

        public override RealizedType UnderlyingType(ModuleContext context) => this;

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
                    new StaticMethodOperatorEmitter(() => new GenericParameter("T"), () => BuiltinType.ArrayBuilder,
                        typeof(ArrayBuilderOps).GetMethod("AddTo"), new OpCode[0])
                }
            },
            new List<(string name, IMethodInvokeFactory invoker)> {
                ("append",
                    new BoundMethodInvokeFactory("Append an element to the array", true, () => BuiltinType.ArrayBuilder,
                        () => new List<RealizedParameter> {new RealizedParameter("element", new GenericParameter("T"))},
                        false, typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Append"))),
                ("result",
                    new BoundMethodInvokeFactory("Build the resulting array", true,
                        () => new ArrayType(new GenericParameter("T")), () => new List<RealizedParameter>(), false,
                        typeof(ArrayBuilder<>), typeof(ArrayBuilder<>).GetMethod("Result"))),
            },
            new List<(string name, IFieldAccessFactory access)> {
                ("length",
                    new BoundPropertyLikeFieldAccessFactory("", () => BuiltinType.Int, typeof(ArrayBuilder<>),
                        typeof(ArrayBuilder<>).GetProperty("Length")))
            }
        );

        public static readonly RealizedType Cell = new BoundType(null, "Cell",
            "Holds a single value that can be mutated at any time", typeof(Cell<>),
            NoOperators,
            NoOperators,
            new List<(string name, IMethodInvokeFactory invoker)> {
                ("set_value",
                    new BoundMethodInvokeFactory("Set the value of the cell", true, () => BuiltinType.Unit,
                        () => new List<RealizedParameter> {new RealizedParameter("value", new GenericParameter("T"))},
                        false, typeof(Cell<>), typeof(Cell<>).GetProperty("Value")?.SetMethod)),
                ("update",
                    new BoundMethodInvokeFactory("Atomically update the value of the cell", true,
                        () => BuiltinType.Cell,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("updater",
                                new FunctionType(false, new List<TO2Type> {new GenericParameter("T")},
                                    new GenericParameter("T")))
                        }, false, typeof(Cell<>), typeof(Cell<>).GetMethod("Update"))),
            },
            new List<(string name, IFieldAccessFactory access)> {
                ("value",
                    new BoundPropertyLikeFieldAccessFactory("", () => new GenericParameter("T"), typeof(Cell<>),
                        typeof(Cell<>).GetProperty("Value")))
            }
        );

        public static TO2Type GetBuiltinType(List<string> namePath, List<TO2Type> typeArguments) {
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
}
