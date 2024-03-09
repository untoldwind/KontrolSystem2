using System;
using System.Collections.Generic;

namespace KontrolSystem.Parsing;

/// <summary>
///     Generic parser.
///     In essence a parser is just a function consuming an input with a successful or failure result.
/// </summary>
public delegate IResult<T> Parser<out T>(IInput input);

public static class ParserExtensions {
    public static IResult<T> TryParse<T>(this Parser<T> parser, string input, string sourceName = "<inline>") {
        return parser(new StringInput(input, sourceName));
    }

    /// <summary>
    ///     Use the parser to parse a string.null
    ///     <param name="input">The input to be parsed</param>
    ///     <param name="sourceName">Name of the source file to be used in Position</param>
    /// </summary>
    public static T Parse<T>(this Parser<T> parser, string input, string sourceName = "<inline>") {
        var result = parser(new StringInput(input, sourceName));

        if (!result.WasSuccessful) throw new ParseException(result.Position, result.Expected);

        return result.Value;
    }

    public static Parser<T> Named<T>(this Parser<T> parser, string expected) {
        return input => {
            var result = parser(input);
            if (!result.WasSuccessful) return Result.Failure<T>(result.Remaining, expected.Yield());
            return result;
        };
    }
}

/// <summary>
///     Exception when parsing fails, containing the position of the input where the failure occured.
/// </summary>
public class ParseException(Position position, List<string> expected) : Exception(
    $"{position}: Expected {string.Join(" or ", expected)}") {
    public readonly List<string> expected = expected;
    public readonly Position position = position;
}

public static class ObjectExt {
    /// <summary>
    ///     Wraps this object instance into an IEnumerable&lt;T&gt;
    ///     consisting of a single item.
    /// </summary>
    /// <typeparam name="T"> Type of the object. </typeparam>
    /// <param name="item"> The instance that will be wrapped. </param>
    /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
    public static IEnumerable<T> Yield<T>(this T item) {
        yield return item;
    }
}
