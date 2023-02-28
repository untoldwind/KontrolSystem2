using System;
using System.Text;
using System.Linq;

namespace KontrolSystem.TO2.Runtime {
    public static class ArrayMethods {
        public static U[] Map<T, U>(T[] source, Func<T, U> mapper) {
            U[] result = new U[source.Length];

            for (int i = 0; i < source.Length; i++)
                result[i] = mapper(source[i]);

            return result;
        }

        public static U[] MapWithIndex<T, U>(T[] source, Func<T, long, U> mapper) {
            U[] result = new U[source.Length];

            for (int i = 0; i < source.Length; i++)
                result[i] = mapper(source[i], i);

            return result;
        }

        public static Option<T> Find<T>(T[] source, Func<T, bool> predicate) {
            foreach (var t in source) {
                if (predicate(t)) return new Option<T>(t);
            }

            return new Option<T>();
        }

        public static bool Exists<T>(T[] source, Func<T, bool> predicate) {
            foreach (var t in source) {
                if (predicate(t)) return true;
            }

            return false;
        }

        public static T[] Filter<T>(T[] source, Func<T, bool> predicate) {
            return source.Where(predicate).ToArray();
        }

        public static string ArrayToString<T>(T[] array) {
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < array.Length; i++) {
                if (i > 0) builder.Append(", ");
                builder.Append(array[i]);
            }

            builder.Append("]");
            return builder.ToString();
        }
    }
}
