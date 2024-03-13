using System;

namespace KontrolSystem.Parsing;

public static partial class Parsers {
    /// <summary>
    ///     A parser that is always successful and does not consume any input.
    /// </summary>
    public static Parser<T> Value<T>(T value) {
        return input => Result.Success(input, value);
    }

    /// <summary>
    ///     Construct a parser that indicates that the given parser
    ///     is optional. The returned parser will succeed on
    ///     any input no matter whether the given parser
    ///     succeeds or not.
    /// </summary>
    public static Parser<IOption<T>> Opt<T>(Parser<T> parser) {
        return input => {
            var result = parser(input);

            if (!result.success) return Result.Success(input, Option.None<T>());

            return Result.Success(result.remaining, Option.Some(result.value));
        };
    }

    /// <summary>
    ///     Construct a parser with two mutually exclusive branches.
    /// </summary>
    public static Parser<IEither<L, R>> Either<L, R>(Parser<L> left, Parser<R> right) {
        return input => {
            var leftResult = left(input);

            if (leftResult.success)
                return Result.Success(leftResult.remaining, Parsing.Either.Left<L, R>(leftResult.value));

            var rightResult = right(input);

            if (rightResult.success)
                return Result.Success(rightResult.remaining, Parsing.Either.Right<L, R>(rightResult.value));

            return Result.Failure<IEither<L, R>>(input, rightResult.expected);
        };
    }

    /// <summary>
    ///     If child parser was successful, return consumed input.
    /// </summary>
    public static Parser<string> Recognize<T>(Parser<T> parser) {
        return input => {
            var result = parser(input);
            if (!result.success) return Result.Failure<string>(result.remaining, result.expected);
            return Result.Success(result.remaining,
                input.Take(result.remaining.Position.position - input.Position.position));
        };
    }

    /// <summary>
    ///     Replace the result of a parsing with the given value.
    /// </summary>
    public static Parser<U> To<T, U>(this Parser<T> parser, U value) {
        return input => parser(input).Select(s => Result.Success(s.remaining, value));
    }

    /// <summary>
    ///     Take the result of parsing, and project it onto a different domain.
    /// </summary>
    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> convert) {
        return input =>
            parser(input).Select(s => Result.Success(s.remaining, convert(s.value)));
    }

    /// <summary>
    ///     Take the result of parsing, and project it onto a different domain with positions.
    /// </summary>
    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, Position, Position, U> convert) {
        return input => {
            var start = input.Position;
            return parser(input).Select(s => Result.Success(s.remaining, convert(s.value, start, s.Position)));
        };
    }

    /// <summary>
    ///     Filter the result of a parser by a predicate.
    /// </summary>
    public static Parser<T> Where<T>(this Parser<T> parser, Predicate<T> predicate, string expected) {
        return input => {
            var result = parser(input);
            if (!result.success) return result;
            if (!predicate(result.value)) return Result.Failure<T>(result.remaining, expected);
            return result;
        };
    }
}
