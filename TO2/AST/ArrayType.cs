using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ArrayType : RealizedType {
    private readonly OperatorCollection allowedSuffixOperators;

    public ArrayType(TO2Type elementType, int dimension = 1) {
        ElementType = dimension > 1 ? new ArrayType(elementType, dimension - 1) : elementType;
        allowedSuffixOperators = new OperatorCollection {
            {
                Operator.Add, new StaticMethodOperatorEmitter(() => this, () => this,
                    typeof(ArrayMethods).GetMethod("Concat"), context => new[] { elementType.UnderlyingType(context) })
            }, {
                Operator.Add, new StaticMethodOperatorEmitter(() => elementType, () => this,
                    typeof(ArrayMethods).GetMethod("Append"), context => new[] { elementType.UnderlyingType(context) })
            }, {
                Operator.AddAssign, new StaticMethodOperatorEmitter(() => this, () => this,
                    typeof(ArrayMethods).GetMethod("Concat"), context => new[] { elementType.UnderlyingType(context) })
            }, {
                Operator.AddAssign, new StaticMethodOperatorEmitter(() => elementType, () => this,
                    typeof(ArrayMethods).GetMethod("Append"), context => new[] { elementType.UnderlyingType(context) })
            }
        };
        DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
            {
                "map", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => new List<RealizedParameter> {
                        new("mapper", new FunctionType(false, new List<TO2Type> {
                            ElementType
                        }, new GenericParameter("U")), "Function to be applied on each element of the array")
                    },
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Map"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "map_with_index", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => new List<RealizedParameter> {
                        new("mapper",
                            new FunctionType(false, new List<TO2Type> { ElementType, BuiltinType.Int },
                                new GenericParameter("U")),
                            "Function to be applied on each element of the array including index of the element")
                    },
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("MapWithIndex"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "filter", new BoundMethodInvokeFactory("Filter the content of the array by a `predicate", true,
                    () => new ArrayType(new GenericParameter("T")),
                    () => new List<RealizedParameter> {
                        new("predictate",
                            new FunctionType(false, new List<TO2Type> { ElementType }, BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array")
                    },
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Filter"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "find", new BoundMethodInvokeFactory("Find first item of the array matching `predicate`", true,
                    () => new OptionType(new GenericParameter("T")),
                    () => new List<RealizedParameter> {
                        new("predictate",
                            new FunctionType(false, new List<TO2Type> { ElementType }, BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array until element is found.")
                    },
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Find"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "exists", new BoundMethodInvokeFactory("Check if an item of the array matches `predicate`", true,
                    () => BuiltinType.Bool,
                    () => new List<RealizedParameter> {
                        new("predictate",
                            new FunctionType(false, new List<TO2Type> { ElementType }, BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array until element is found.")
                    },
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Exists"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "slice",
                new BoundMethodInvokeFactory("Get a slice of the array", true, () => new ArrayType(ElementType),
                    () => new List<RealizedParameter> {
                        new("start", BuiltinType.Int, "Start index of the slice (inclusive)"),
                        new("end", BuiltinType.Int, "End index of the slice (exclusive)", new IntDefaultValue(-1))
                    }, false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("Slice"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "sort",
                new BoundMethodInvokeFactory("Sort the array (if possible) and returns new sorted array", true,
                    () => new ArrayType(ElementType),
                    () => new List<RealizedParameter>(), false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("Sort"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "sort_by",
                new BoundMethodInvokeFactory(
                    "Sort the array by value extracted from items. Sort value can be other number or string",
                    true, () => new ArrayType(ElementType),
                    () => new List<RealizedParameter> {
                        new("value",
                            new FunctionType(false, new List<TO2Type> { ElementType }, new GenericParameter("U")),
                            "Function to be applied on each element, array will be sorted by result of this function")
                    }, false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("SortBy"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "sort_with",
                new BoundMethodInvokeFactory(
                    "Sort the array with explicit comparator. Comparator should return -1 for less, 0 for equal and 1 for greater",
                    true, () => new ArrayType(ElementType),
                    () => new List<RealizedParameter> {
                        new("comarator",
                            new FunctionType(false, new List<TO2Type> { ElementType, ElementType }, BuiltinType.Int),
                            "Compare two elements of the array to each other, `-1` less then, `0` equal, `1` greater than")
                    }, false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("SortWith"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "reduce",
                new BoundMethodInvokeFactory("Reduce array by an operation", true, () => new GenericParameter("U"),
                    () => new List<RealizedParameter> {
                        new("initial", new GenericParameter("U"), "Initial value of the accumulator"),
                        new("reducer", new FunctionType(false, new List<TO2Type> {
                                new GenericParameter("U"),
                                ElementType
                            }, new GenericParameter("U")),
                            "Combines accumulator with each element of the array and returns new accumulator value")
                    }, false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("Reduce"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "reverse",
                new BoundMethodInvokeFactory("Reverse the order of the array", true, () => new ArrayType(ElementType),
                    () => new List<RealizedParameter>(), false, typeof(ArrayMethods),
                    typeof(ArrayMethods).GetMethod("Reverse"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }, {
                "to_string", new BoundMethodInvokeFactory("Get string representation of the array", true,
                    () => BuiltinType.String,
                    () => new List<RealizedParameter>(),
                    false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("ArrayToString"),
                    context => ("T", ElementType.UnderlyingType(context)).Yield())
            }
        };
        DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
            {
                "length",
                new InlineFieldAccessFactory("Length of the array, i.e. number of elements in the array.",
                    () => BuiltinType.Int, REPLArrayLength, OpCodes.Ldlen, OpCodes.Conv_I8)
            }
        };
    }

    public TO2Type ElementType { get; }

    public override string Name => $"{ElementType}[]";

    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override bool IsValid(ModuleContext context) {
        return ElementType.IsValid(context);
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return new ArrayType(ElementType.UnderlyingType(context));
    }

    public override Type GeneratedType(ModuleContext context) {
        return ElementType.GeneratedType(context).MakeArrayType();
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
        return allowedSuffixOperators;
    }

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) {
        switch (indexSpec.indexType) {
        case IndexSpecType.Single:
            var underlyingElement = ElementType.UnderlyingType(context);

            return new InlineArrayIndexAccessEmitter(underlyingElement, indexSpec.start);
        default:
            return null;
        }
    }

    public override IForInSource ForInSource(ModuleContext context, TO2Type? typeHint) {
        return new ArrayForInSource(GeneratedType(context), ElementType.UnderlyingType(context));
    }

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        return new ArrayType(ElementType.UnderlyingType(context).FillGenerics(context, typeArguments));
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        var concreteArray = concreteType as ArrayType;
        if (concreteArray == null) return Enumerable.Empty<(string name, RealizedType type)>();
        return ElementType.InferGenericArgument(context, concreteArray.ElementType.UnderlyingType(context));
    }

    public static IREPLValue REPLArrayLength(Node node, IREPLValue target) {
        if (target.Value is Array a) return new REPLInt(a.Length);

        throw new REPLException(node, $"Get array length from a non-array: {target.Type.Name}");
    }

    public override IREPLValue REPLCast(object? value) {
        if (value is Array a)
            return new REPLArray(this, a);

        throw new REPLException(new Position("Intern"), new Position("Intern"),
            $"{value?.GetType()} can not be cast to REPLArray");
    }
}

public class ArrayForInSource : IForInSource {
    private readonly Type arrayType;
    private ILocalRef? arrayRef;

    private ILocalRef? currentIndex;

    public ArrayForInSource(Type arrayType, RealizedType elementType) {
        this.arrayType = arrayType;
        ElementType = elementType;
    }

    public RealizedType ElementType { get; }

    public void EmitInitialize(IBlockContext context) {
        arrayRef = context.DeclareHiddenLocal(arrayType);
        arrayRef.EmitStore(context);
        currentIndex = context.DeclareHiddenLocal(typeof(int));
        context.IL.Emit(OpCodes.Ldc_I4_M1);
        currentIndex.EmitStore(context);
    }

    public void EmitCheckDone(IBlockContext context, LabelRef loop) {
        currentIndex!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldc_I4_1);
        context.IL.Emit(OpCodes.Add);
        context.IL.Emit(OpCodes.Dup);
        currentIndex!.EmitStore(context);
        arrayRef!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldlen);
        context.IL.Emit(OpCodes.Conv_I4);
        context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
    }

    public void EmitNext(IBlockContext context) {
        arrayRef!.EmitLoad(context);
        currentIndex!.EmitLoad(context);
        if (ElementType == BuiltinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
        else if (ElementType == BuiltinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
        else if (ElementType == BuiltinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
        else context.IL.Emit(OpCodes.Ldelem, ElementType.GeneratedType(context.ModuleContext));
    }
}
