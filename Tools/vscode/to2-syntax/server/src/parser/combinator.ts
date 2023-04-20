import { Position } from "vscode-languageserver-textdocument";
import { Input, Parser, ParserFailure, ParserSuccess } from ".";

export function value<T>(value: T): Parser<T> {
  return (input: Input) => new ParserSuccess<T>(input, value);
}

export function recognize<T>(parser: Parser<T>): Parser<string> {
  return (input: Input) => {
    const result = parser(input);
    if (result.success) {
      return new ParserSuccess(
        result.remaining,
        input.take(result.remaining.offset - input.offset)
      );
    }
    return new ParserFailure(result.remaining, result.expected);
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
    return new ParserFailure(result.remaining, result.expected);
  };
}

export function where<T>(
  parser: Parser<T>,
  predicate: (result: T) => boolean,
  expected: string
): Parser<T> {
  return (input: Input) => {
    const result = parser(input);
    if (result.success && !predicate(result.result))
      return new ParserFailure(result.remaining, expected);
    return result;
  };
}

export function map<T, U>(
  parser: Parser<T>,
  convert: (result: T, start: Position, end: Position) => U
): Parser<U> {
  return (input: Input) =>
    parser(input).select(
      (s) =>
        new ParserSuccess(
          s.remaining,
          convert(s.result, input.position, s.remaining.position)
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
