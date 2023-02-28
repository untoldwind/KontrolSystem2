using System;

namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// A parser that is always successful and does not consume any input.
        /// </summary>
        public static Parser<T> Value<T>(T value) => input => Result.Success(input, value);

        /// <summary>
        /// Construct a parser that indicates that the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        public static Parser<IOption<T>> Opt<T>(Parser<T> parser) => input => {
            IResult<T> result = parser(input);

            if (!result.WasSuccessful) return Result.Success(input, Option.None<T>());

            return Result.Success(result.Remaining, Option.Some(result.Value));
        };

        /// <summary>
        /// Construct a parser with two mutually exclusive branches.
        /// </summary>
        public static Parser<IEither<L, R>> Either<L, R>(Parser<L> left, Parser<R> right) => input => {
            IResult<L> leftResult = left(input);

            if (leftResult.WasSuccessful)
                return Result.Success(leftResult.Remaining, Parsing.Either.Left<L, R>(leftResult.Value));

            IResult<R> rightResult = right(input);

            if (rightResult.WasSuccessful)
                return Result.Success(rightResult.Remaining, Parsing.Either.Right<L, R>(rightResult.Value));

            return Result.Failure<IEither<L, R>>(input, rightResult.Expected);
        };

        /// <summary>
        /// If child parser was successful, return consumed input.
        /// </summary>
        public static Parser<string> Recognize<T>(Parser<T> parser) => input => {
            IResult<T> result = parser(input);
            if (!result.WasSuccessful) return Result.Failure<string>(result.Remaining, result.Expected);
            return Result.Success(result.Remaining,
                input.Take(result.Remaining.Position.position - input.Position.position));
        };

        /// <summary>
        /// Replace the result of a parsing with the given value.
        /// </summary>
        public static Parser<U> To<T, U>(this Parser<T> parser, U value) =>
            input => parser(input).Select(s => Result.Success(s.Remaining, value));

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> convert) => input =>
            parser(input).Select(s => Result.Success(s.Remaining, convert(s.Value)));

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain with positions.
        /// </summary>
        public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, Position, Position, U> convert) => input =>
            parser(input).Select(s => Result.Success(s.Remaining, convert(s.Value, input.Position, s.Position)));

        /// <summary>
        /// Filter the result of a parser by a predicate.
        /// </summary>
        public static Parser<T> Where<T>(this Parser<T> parser, Predicate<T> predicate, string expected) => input => {
            IResult<T> result = parser(input);
            if (!result.WasSuccessful) return result;
            if (!predicate(result.Value)) return Result.Failure<T>(result.Remaining, expected);
            return result;
        };
    }
}
