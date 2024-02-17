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

                if (result.WasSuccessful) return result;

                var longestAt = longest.Position.position;
                var errorAt = result.Remaining.Position.position;
                if (errorAt == longestAt) {
                    expected = expected.Concat(result.Expected);
                } else if (errorAt > longestAt) {
                    longest = result.Remaining;
                    expected = result.Expected;
                }
            }

            return Result.Failure<T>(longest, expected);
        };
    }
}
