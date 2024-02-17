using System;

namespace KontrolSystem.TO2.Runtime;

public static class StringMethods {
    public static long IndexOf(string source, string other, long startIndex) {
        return source.IndexOf(other, (int)startIndex, StringComparison.InvariantCulture);
    }

    public static string Slice(string source, long start, long end) {
        start = start < 0 ? 0 : start;
        end = end < 0 || end > source.Length ? source.Length : end;
        if (start >= end)
            return "";
        return source.Substring((int)start, (int)(end - start));
    }

    public static string[] Split(string source, string separator) {
        return source.Split(separator);
    }
}
