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

    public static string Ellipsis(string source, long maxLength) {
        if (source.Length <= maxLength) return source;

        return source[..((int)maxLength - 3)] + "...";
    }

    public static Result<long> ParseInt(string source) {
        if (Int64.TryParse(source, out var value)) {
            return Result.Ok(value);
        }

        return Result.Err<long>($"'{source}' is not a valid integer");
    }

    public static Result<double> ParseDouble(string source) {
        if (Double.TryParse(source, out var value)) {
            return Result.Ok(value);
        }

        return Result.Err<double>($"'{source}' is not a valid floating point");
    }
}
