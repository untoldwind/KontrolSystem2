using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// Create a parser from an array of alternatives to try.
        /// The alternatives will be tried in order of the array until one succeeds.
        /// </summary>
        public static Parser<T> Alt<T>(params Parser<T>[] alternatives) => input => {
            IInput longest = input;
            IEnumerable<string> expected = Enumerable.Empty<string>();

            foreach (Parser<T> alternative in alternatives) {
                IResult<T> result = alternative(input);

                if (result.WasSuccessful) return result;

                int longestAt = longest.Position.position;
                int errorAt = result.Remaining.Position.position;
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
