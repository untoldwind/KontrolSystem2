import { Position } from "vscode-languageserver-textdocument";
import { Input, Parser, ParserFailure, ParserResult, ParserSuccess } from ".";
import { seq } from "./sequence";

export function manyM_N<T>(
  minCount: number | undefined,
  maxCount: number | undefined,
  item: Parser<T>,
  description: string
): Parser<T[]> {
  return (input: Input) => {
    let remaining = input;
    let result: T[] = [];
    let itemResult: ParserResult<T> = item(input);

    while (itemResult.success) {
      if (remaining.offset === itemResult.remaining.offset) break;

      result.push(itemResult.result);
      remaining = itemResult.remaining;

      itemResult = item(remaining);
    }

    if (minCount && result.length < minCount) {
      return new ParserFailure(
        remaining,
        `Expected at least ${minCount} ${description}`
      );
    }
    if (maxCount && result.length > maxCount) {
      return new ParserFailure(
        remaining,
        `Expected at most ${maxCount} ${description}`
      );
    }

    return new ParserSuccess(remaining, result);
  };
}

export function many0<T>(item: Parser<T>, description: string): Parser<T[]> {
  return manyM_N(undefined, undefined, item, description);
}

export function many1<T>(item: Parser<T>, description: string): Parser<T[]> {
  return manyM_N(1, undefined, item, description);
}

export function delimitedM_N<T, D>(
  minCount: number | undefined,
  maxCount: number | undefined,
  item: Parser<T>,
  delimiter: Parser<D>,
  description: string
): Parser<T[]> {
  return (input: Input) => {
    let remaining = input;
    let result: T[] = [];
    let itemResult: ParserResult<T> = item(input);

    while (itemResult.success) {
      if (remaining.offset === itemResult.remaining.offset) break;

      result.push(itemResult.result);
      remaining = itemResult.remaining;

      const delimiterResult = delimiter(remaining);
      if (!delimiterResult.success) break;

      itemResult = item(delimiterResult.remaining);
    }

    if (minCount && result.length < minCount) {
      return new ParserFailure(
        remaining,
        `Expected at least ${minCount} ${description}`
      );
    }
    if (maxCount && result.length > maxCount) {
      return new ParserFailure(
        remaining,
        `Expected at most ${maxCount} ${description}`
      );
    }

    return new ParserSuccess(remaining, result);
  };
}

export function delimited0<T, D>(
  item: Parser<T>,
  delimiter: Parser<D>,
  description: string
): Parser<T[]> {
  return delimitedM_N(undefined, undefined, item, delimiter, description);
}

export function delimited1<T, D>(
  item: Parser<T>,
  delimiter: Parser<D>,
  description: string
): Parser<T[]> {
  return delimitedM_N(1, undefined, item, delimiter, description);
}

export function fold0<T, S>(
  initial: Parser<T>,
  suffix: Parser<S>,
  combine: (collect: T, current: S, start: Position, end: Position) => T
): Parser<T> {
  return (input: Input) => {
    let result = initial(input);
    if (!result.success) return result;

    let suffixResult = suffix(result.remaining);
    while (suffixResult.success) {
      if (suffixResult.remaining.offset === result.remaining.offset) break;

      result = new ParserSuccess(
        suffixResult.remaining,
        combine(
          result.result,
          suffixResult.result,
          result.remaining.position,
          suffixResult.remaining.position
        )
      );
      suffixResult = suffix(suffixResult.remaining);
    }

    return result;
  };
}

export function chain<T, OP>(
  operantParser: Parser<T>,
  opParser: Parser<OP>,
  apply: (left: T, op: OP, right: T, start: Position, end: Position) => T
) {
  const restParser = seq([opParser, operantParser]);

  return (input: Input) => {
    const firstResult = operantParser(input);

    if (!firstResult.success) return firstResult;

    let remaining = firstResult.remaining;
    let result = firstResult.result;

    let restResult = restParser(remaining);

    while (restResult.success) {
      if (remaining.offset === restResult.remaining.offset) break;

      var [op, operant] = restResult.result;

      result = apply(
        result,
        op,
        operant,
        remaining.position,
        restResult.remaining.position
      );
      remaining = restResult.remaining;

      restResult = restParser(remaining);
    }

    return new ParserSuccess(remaining, result);
  };
}
