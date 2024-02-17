using System;

namespace KontrolSystem.TO2.Runtime;

public struct Range {
    public readonly long from;
    public readonly long to;

    public Range(long from, long to) {
        this.from = from;
        this.to = to;
    }

    public long Length => to < from ? 0 : to - from;

    public T[] Map<T>(Func<long, T> mapper) {
        if (to < from) return new T[0];

        var result = new T[to - from];

        for (var i = from; i < to; i++)
            result[i - from] = mapper(i);

        return result;
    }

    public long[] Reverse() {
        if (to < from) return new long[0];

        var result = new long[to - from];

        for (long i = 0; i < to - from; i++) result[i] = to - i - 1;

        return result;
    }

    public U Reduce<U>(U initial, Func<U, long, U> reducer) {
        var result = initial;
        for (var i = from; i < to; i++) result = reducer(result, i);
        return result;
    }

    public string RangeToString() {
        return $"{from}..{to}";
    }
}
