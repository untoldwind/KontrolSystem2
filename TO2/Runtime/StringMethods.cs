using System;

namespace KontrolSystem.TO2.Runtime {
    public static class StringMethods {
        public static long IndexOf(string source, string other, long startIndex) =>
            source.IndexOf(other, (int)startIndex, StringComparison.InvariantCulture);
    }
}
