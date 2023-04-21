import { Input, InputPosition, Parser, ParserFailure, ParserSuccess } from ".";

export function value<T>(value: T): Parser<T> {
  return (input: Input) => new ParserSuccess<T>(input, value);
}

export function recognize<T>(parser: Parser<T>): Parser<string> {
  return (input: Input) => {
    const result = parser(input);
    if (result.success) {
      return new ParserSuccess(
        result.remaining,
        input.take(result.remaining.position.offset - input.position.offset)
      );
    }
    return new ParserFailure(result.remaining, result.expected, "");
  };
}

export function recognizeAs<T, U>(
  parser: Parser<T>,
  replacement: U
): Parser<U> {
  return (input: Input) => {
    const result = parser(input);
    if (result.success) {
      return new ParserSuccess(result.remaining, replacement);
    }
    return new ParserFailure(result.remaining, result.expected, replacement);
  };
}

export function where<T>(
  parser: Parser<T>,
  predicate: (result: T) => boolean,
  expected: string
): Parser<T> {
  return (input: Input) => {
    const result = parser(input);
    if (result.success && !predicate(result.value))
      return new ParserFailure(result.remaining, expected, result.value);
    return result;
  };
}

export function map<T, U>(
  parser: Parser<T>,
  convert: (result: T, start: InputPosition, end: InputPosition) => U
): Parser<U> {
  return (input: Input) =>
    parser(input).select(
      (s) =>
        new ParserSuccess(
          s.remaining,
          convert(s.value, input.position, s.remaining.position)
        )
    );
}

export function opt<T>(parser: Parser<T>): Parser<T | undefined> {
  return (input: Input) => {
    const result = parser(input);

    if (result.success) return result;

    return new ParserSuccess(input, undefined);
  };
}

export function either<L, R>(left: Parser<L>, right: Parser<R>): Parser<L | R> {
  return (input: Input) => {
    const leftResult = left(input);

    if (leftResult.success) return leftResult;

    return right(input);
  };
}
