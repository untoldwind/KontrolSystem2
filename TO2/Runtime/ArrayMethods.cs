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

        public static T[] Reverse<T>(T[] source) {
            return source.Reverse().ToArray();
        }

        public static T[] Concat<T>(T[] array1, T[] array2) {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

        public static T[] Append<T>(T[] array, T element) {
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = element;
            return result;
        }

        public static string ArrayToString<T>(T[] array) {
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < array.Length; i++) {
                if (i > 0) builder.Append(", ");
                if (array[i] is Array subArray) {
                    builder.Append(ArrayToString(subArray.Cast<object>().ToArray()));
                } else if (array[i] is bool b) {
                    builder.Append(FormatUtils.BoolToString(b));
                } else if (array[i] is long l) {
                    builder.Append(FormatUtils.IntToString(l));
                } else if (array[i] is double d) {
                    builder.Append(FormatUtils.FloatToString(d));
                } else {
                    builder.Append(array[i]);
                }
            }

            builder.Append("]");
            return builder.ToString();
        }
    }
}
