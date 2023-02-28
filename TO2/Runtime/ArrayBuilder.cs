using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime {
    public class ArrayBuilder<T> {
        private readonly List<T> elements;

        public ArrayBuilder(long capacity) => elements = new List<T>((int)capacity);

        public long Length => elements.Count;

        public ArrayBuilder<T> Append(T element) {
            elements.Add(element);
            return this;
        }

        public T[] Result() => elements.ToArray();
    }

    public static class ArrayBuilder {
        public static ArrayBuilder<T> Create<T>(long capacity) => new ArrayBuilder<T>(capacity);
    }

    public static class ArrayBuilderOps {
        public static ArrayBuilder<T> AddTo<T>(ArrayBuilder<T> builder, T element) => builder.Append(element);
    }
}
