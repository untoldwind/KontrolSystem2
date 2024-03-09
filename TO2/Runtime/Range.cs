using System;
using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime;

public readonly struct Range(long from, long to) {
    public readonly long from = from;
    public readonly long to = to;

    public readonly long Length => to < from ? 0 : to - from;

    public readonly T[] Map<T>(Func<long, T> mapper) {
        if (to < from) return [];

        var result = new T[to - from];

        for (var i = from; i < to; i++)
            result[i - from] = mapper(i);

        return result;
    }

    public readonly T[] FlatMap<T>(Func<long, T[]> mapper) {
        if (to < from) return [];
        var result = new List<T>((int)(to - from));

        for (var i = from; i < to; i++)
            foreach (var item in mapper(i))
                result.Add(item);

        return result.ToArray();
    }

    public readonly T[] FilterMap<T>(Func<long, Option<T>> mapper) {
        if (to < from) return [];

        var result = new List<T>((int)(to - from));

        for (var i = from; i < to; i++) {
            var item = mapper(i);

            if (item.defined) result.Add(item.value);
        }

        return result.ToArray();
    }

    public readonly long[] Reverse() {
        if (to < from) return [];

        var result = new long[to - from];

        for (long i = 0; i < to - from; i++) result[i] = to - i - 1;

        return result;
    }

    public readonly U Reduce<U>(U initial, Func<U, long, U> reducer) {
        var result = initial;
        for (var i = from; i < to; i++) result = reducer(result, i);
        return result;
    }

    public readonly string RangeToString() {
        return $"{from}..{to}";
    }
}
