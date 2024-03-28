using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime;

public static class EnumerableMethods {
    public static U[] Map<T, U>(IEnumerable<T> source, Func<T, U> mapper) {
        return source.Select(mapper).ToArray();
    }

    public static U[] MapWithIndex<T, U>(IEnumerable<T> source, Func<T, long, U> mapper) {
        return source.Select((item, index) => mapper(item, index)).ToArray();
    }

    public static U[] FlatMap<T, U>(IEnumerable<T> source, Func<T, U[]> mapper) {
        return source.SelectMany(mapper).ToArray();
    }

    public static U[] FilterMap<T, U>(IEnumerable<T> source, Func<T, Option<U>> mapper) {
        return source.SelectMany(item => mapper(item).AsEnumerable()).ToArray();
    }

    public static Option<T> Find<T>(IEnumerable<T> source, Func<T, bool> predicate) {
        foreach (var t in source)
            if (predicate(t))
                return new Option<T>(t);

        return new Option<T>();
    }

    public static bool Exists<T>(IEnumerable<T> source, Func<T, bool> predicate) {
        foreach (var t in source)
            if (predicate(t))
                return true;

        return false;
    }

    public static T[] Filter<T>(IEnumerable<T> source, Func<T, bool> predicate) {
        return source.Where(predicate).ToArray();
    }

    public static T[] Reverse<T>(IEnumerable<T> source) {
        return source.Reverse().ToArray();
    }

    public static U Reduce<T, U>(IEnumerable<T> source, U initial, Func<U, T, U> reducer) {
        return source.Aggregate(initial, reducer);
    }

    private static readonly Type EnumerableMethodsType = typeof(EnumerableMethods);

    public static IEnumerable<(string name, IMethodInvokeFactory invoker)> MethodInvokers(Type implementationType) {
        var elementType = implementationType.GetInterface("IEnumerable`1").GenericTypeArguments[0];
        return [
            (
                "map", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => [
                        new("mapper",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)],
                                new GenericParameter("U")),
                            "Function to be applied on each element of the array")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("Map"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "map_with_index", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => [
                        new("mapper",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType), BuiltinType.Int],
                                new GenericParameter("U")),
                            "Function to be applied on each element of the array including index of the element")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("MapWithIndex"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "flat_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => [
                        new("mapper",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)],
                                new ArrayType(new GenericParameter("U"))),
                            "Function to be applied on each element of the array")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("FlatMap"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "filter", new BoundMethodInvokeFactory("Filter the content of the array by a `predicate", true,
                    () => new ArrayType(new GenericParameter("T")),
                    () => [
                        new("predicate",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)], BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("Filter"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "filter_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                    () => new ArrayType(new GenericParameter("U")),
                    () => [
                        new("mapper",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)],
                                new OptionType(new GenericParameter("U"))),
                            "Function to be applied on each element of the array")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("FilterMap"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "find", new BoundMethodInvokeFactory("Find first item of the array matching `predicate`", true,
                    () => new OptionType(new GenericParameter("T")),
                    () => [
                        new("predicate",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)], BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array until element is found.")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("Find"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "exists", new BoundMethodInvokeFactory("Check if an item of the array matches `predicate`", true,
                    () => BuiltinType.Bool,
                    () => [
                        new("predicate",
                            new FunctionType(false, [BindingGenerator.MapNativeType(elementType)], BuiltinType.Bool),
                            "Predicate function/check to be applied on each element of the array until element is found.")
                    ],
                    false, EnumerableMethodsType, EnumerableMethodsType.GetMethod("Exists"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "reduce",
                new BoundMethodInvokeFactory("Reduce array by an operation", true, () => new GenericParameter("U"),
                    () => [
                        new("initial", new GenericParameter("U"), "Initial value of the accumulator"),
                        new("reducer", new FunctionType(false, [
                                new GenericParameter("U"),
                            BindingGenerator.MapNativeType(elementType)
                            ], new GenericParameter("U")),
                            "Combines accumulator with each element of the array and returns new accumulator value")
                    ], false, EnumerableMethodsType,
                    EnumerableMethodsType.GetMethod("Reduce"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            ),
            (
                "reverse",
                new BoundMethodInvokeFactory("Reverse the order of the array", true,
                    () => new ArrayType(BindingGenerator.MapNativeType(elementType)),
                    () => [], false, EnumerableMethodsType,
                    EnumerableMethodsType.GetMethod("Reverse"),
                    context => [("T", BindingGenerator.MapNativeType(elementType))])
            )
        ];
    }
}
