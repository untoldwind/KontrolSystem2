using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime;

public static class ArrayMethods {
    public static U[] Map<T, U>(T[] source, Func<T, U> mapper) {
        var result = new U[source.Length];

        for (var i = 0; i < source.Length; i++)
            result[i] = mapper(source[i]);

        return result;
    }

    public static U[] MapWithIndex<T, U>(T[] source, Func<T, long, U> mapper) {
        var result = new U[source.Length];

        for (var i = 0; i < source.Length; i++)
            result[i] = mapper(source[i], i);

        return result;
    }

    public static U[] FlatMap<T, U>(T[] source, Func<T, U[]> mapper) {
        var result = new List<U>(source.Length);

        for (var i = 0; i < source.Length; i++)
            foreach (var item in mapper(source[i]))
                result.Add(item);

        return [.. result];
    }

    public static U[] FilterMap<T, U>(T[] source, Func<T, Option<U>> mapper) {
        var result = new List<U>(source.Length);

        for (var i = 0; i < source.Length; i++) {
            var item = mapper(source[i]);

            if (item.defined) result.Add(item.value);
        }

        return [.. result];
    }

    public static Option<T> Find<T>(T[] source, Func<T, bool> predicate) {
        foreach (var t in source)
            if (predicate(t))
                return new Option<T>(t);

        return new Option<T>();
    }

    public static bool Exists<T>(T[] source, Func<T, bool> predicate) {
        foreach (var t in source)
            if (predicate(t))
                return true;

        return false;
    }

    public static T[] Filter<T>(T[] source, Func<T, bool> predicate) {
        return source.Where(predicate).ToArray();
    }

    public static T[] Reverse<T>(T[] source) {
        return source.Reverse().ToArray();
    }

    public static T[] Concat<T>(T[] array1, T[] array2) {
        var result = new T[array1.Length + array2.Length];
        array1.CopyTo(result, 0);
        array2.CopyTo(result, array1.Length);
        return result;
    }

    public static T[] Append<T>(T[] array, T element) {
        var result = new T[array.Length + 1];
        array.CopyTo(result, 0);
        result[array.Length] = element;
        return result;
    }

    public static T[] Sort<T>(T[] array) {
        var result = new T[array.Length];
        array.CopyTo(result, 0);
        try {
            Array.Sort(result);
        } catch (InvalidOperationException) {
            // Ignore
        }

        return result;
    }

    public static T[] SortBy<T, U>(T[] array, Func<T, U> value) {
        try {
            return [.. array.OrderBy(value)];
        } catch (InvalidOperationException) {
            return array;
        }
    }

    public static T[] SortWith<T>(T[] array, Func<T, T, long> comparer) {
        var result = new T[array.Length];
        array.CopyTo(result, 0);
        Array.Sort(result, (a, b) => (int)comparer(a, b));

        return result;
    }

    public static U Reduce<T, U>(T[] source, U initial, Func<U, T, U> reducer) {
        var result = initial;
        for (var i = 0; i < source.Length; i++)
            result = reducer(result, source[i]);
        return result;
    }

    public static T[] Slice<T>(T[] source, long start, long end) {
        start = start < 0 ? 0 : start;
        end = end < 0 || end > source.Length ? source.Length : end;
        if (start >= end)
            return [];
        var result = new T[end - start];
        Array.Copy(source, start, result, 0, end - start);
        return result;
    }

    public static string ArrayToString<T>(T[] array) {
        var builder = new StringBuilder("[");

        for (var i = 0; i < array.Length; i++) {
            if (i > 0) builder.Append(", ");
            if (array[i] is Array subArray)
                builder.Append(ArrayToString(subArray.Cast<object>().ToArray()));
            else if (array[i] is bool b)
                builder.Append(FormatUtils.BoolToString(b));
            else if (array[i] is long l)
                builder.Append(FormatUtils.IntToString(l));
            else if (array[i] is double d)
                builder.Append(FormatUtils.FloatToString(d));
            else
                builder.Append(array[i]);
        }

        builder.Append("]");
        return builder.ToString();
    }

    public static Array DeepClone(Array source) {
        Array clone = Array.CreateInstance(source.GetType().GetElementType()!, source.Length);

        for (var i = 0; i < source.Length; i++) {
            var item = source.GetValue(i);
            if (item is Array subArray) {
                clone.SetValue(DeepClone(subArray), i);
            } else {
                clone.SetValue(item, i);
            }
        }

        return clone;
    }

    private static readonly Type ArrayMethodsType = typeof(ArrayMethods);

    public static IEnumerable<(string name, IMethodInvokeFactory invoker)> MethodInvokers(TO2Type elementType) => [
        (
            "map", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("U")),
                () => [
                    new("mapper", new FunctionType(false, [elementType], new GenericParameter("U")),
                        "Function to be applied on each element of the array")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("Map"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "map_with_index", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("U")),
                () => [
                    new("mapper",
                        new FunctionType(false, [elementType, BuiltinType.Int],
                            new GenericParameter("U")),
                        "Function to be applied on each element of the array including index of the element")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("MapWithIndex"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "flat_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("U")),
                () => [
                    new("mapper", new FunctionType(false, [elementType], new ArrayType(new GenericParameter("U"))),
                        "Function to be applied on each element of the array")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("FlatMap"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "filter", new BoundMethodInvokeFactory("Filter the content of the array by a `predicate", true,
                () => new ArrayType(new GenericParameter("T")),
                () => [
                    new("predicate",
                        new FunctionType(false, [elementType], BuiltinType.Bool),
                        "Predicate function/check to be applied on each element of the array")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("Filter"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "filter_map", new BoundMethodInvokeFactory("Map the content of the array", true,
                () => new ArrayType(new GenericParameter("U")),
                () => [
                    new("mapper", new FunctionType(false, [elementType], new OptionType(new GenericParameter("U"))),
                        "Function to be applied on each element of the array")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("FilterMap"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "find", new BoundMethodInvokeFactory("Find first item of the array matching `predicate`", true,
                () => new OptionType(new GenericParameter("T")),
                () => [
                    new("predicate",
                        new FunctionType(false, [elementType], BuiltinType.Bool),
                        "Predicate function/check to be applied on each element of the array until element is found.")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("Find"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "exists", new BoundMethodInvokeFactory("Check if an item of the array matches `predicate`", true,
                () => BuiltinType.Bool,
                () => [
                    new("predicate",
                        new FunctionType(false, [elementType], BuiltinType.Bool),
                        "Predicate function/check to be applied on each element of the array until element is found.")
                ],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("Exists"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "slice",
            new BoundMethodInvokeFactory("Get a slice of the array", true, () => new ArrayType(elementType),
                () => [
                    new("start", BuiltinType.Int, "Start index of the slice (inclusive)"),
                    new("end", BuiltinType.Int, "End index of the slice (exclusive)", new IntDefaultValue(-1))
                ], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("Slice"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "sort",
            new BoundMethodInvokeFactory("Sort the array (if possible) and returns new sorted array", true,
                () => new ArrayType(elementType),
                () => [], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("Sort"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "sort_by",
            new BoundMethodInvokeFactory(
                "Sort the array by value extracted from items. Sort value can be other number or string",
                true, () => new ArrayType(elementType),
                () => [
                    new("value",
                        new FunctionType(false, [elementType], new GenericParameter("U")),
                        "Function to be applied on each element, array will be sorted by result of this function")
                ], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("SortBy"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "sort_with",
            new BoundMethodInvokeFactory(
                "Sort the array with explicit comparator. Comparator should return -1 for less, 0 for equal and 1 for greater",
                true, () => new ArrayType(elementType),
                () => [
                    new("comparator",
                        new FunctionType(false, [elementType, elementType], BuiltinType.Int),
                        "Compare two elements of the array to each other, `-1` less then, `0` equal, `1` greater than")
                ], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("SortWith"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "reduce",
            new BoundMethodInvokeFactory("Reduce array by an operation", true, () => new GenericParameter("U"),
                () => [
                    new("initial", new GenericParameter("U"), "Initial value of the accumulator"),
                    new("reducer", new FunctionType(false, [
                            new GenericParameter("U"),
                        elementType
                        ], new GenericParameter("U")),
                        "Combines accumulator with each element of the array and returns new accumulator value")
                ], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("Reduce"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "reverse",
            new BoundMethodInvokeFactory("Reverse the order of the array", true, () => new ArrayType(elementType),
                () => [], false, ArrayMethodsType,
                ArrayMethodsType.GetMethod("Reverse"),
                context => [("T", elementType.UnderlyingType(context))])
        ),
        (
            "to_string", new BoundMethodInvokeFactory("Get string representation of the array", true,
                () => BuiltinType.String,
                () => [],
                false, ArrayMethodsType, ArrayMethodsType.GetMethod("ArrayToString"),
                context => [("T", elementType.UnderlyingType(context))])
        )
    ];
}
