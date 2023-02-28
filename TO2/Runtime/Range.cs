using System;

namespace KontrolSystem.TO2.Runtime {
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

            T[] result = new T[to - from];

            for (long i = from; i < to; i++)
                result[i - from] = mapper(i);

            return result;
        }
    }
}
