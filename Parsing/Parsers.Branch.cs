using System.Linq;

namespace KontrolSystem.Parsing;

public static partial class Parsers {
    /// <summary>
    ///     Create a parser from an array of alternatives to try.
    ///     The alternatives will be tried in order of the array until one succeeds.
    /// </summary>
    public static Parser<T> Alt<T>(params Parser<T>[] alternatives) {
        return input => {
            var longest = input;
            var expected = Enumerable.Empty<string>();

            foreach (var alternative in alternatives) {
                var result = alternative(input);

                if (result.success) return result;

                var longestAt = longest.Position.position;
                var errorAt = result.remaining.Position.position;
                if (errorAt == longestAt) {
                    expected = expected.Concat(result.expected);
                } else if (errorAt > longestAt) {
                    longest = result.remaining;
                    expected = result.expected;
                }
            }

            return Result.Failure<T>(longest, expected);
        };
    }

    public static Parser<T> Select<T>(params (char, T)[] alternatives) {
        var expected = alternatives.Select(t => $"'{t.Item1}'");

        return input => {
            if (input.Available > 0) {
                var ch = input.Current;
                foreach (var (prefix, value) in alternatives) {
                    if (prefix == ch) return Result.Success(input.Advance(1), value);
                }
            }

            return Result.Failure<T>(input, expected);
        };
    }

    public static Parser<T> Select<T>(params (string, T)[] alternatives) {
        var expected = alternatives.Select(t => $"'{t.Item1}'");

        return input => {
            if (input.Available > 0) {
                foreach (var (prefix, value) in alternatives) {
                    if (input.Match(prefix)) return Result.Success(input.Advance(prefix.Length), value);
                }
            }

            return Result.Failure<T>(input, expected);
        };
    }

}
