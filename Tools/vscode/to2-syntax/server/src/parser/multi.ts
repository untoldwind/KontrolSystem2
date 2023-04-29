import {
  Input,
  InputPosition,
  Parser,
  ParserFailure,
  ParserResult,
  ParserSuccess,
} from ".";
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
      if (remaining.position.offset === itemResult.remaining.position.offset)
        break;

      result.push(itemResult.value);
      remaining = itemResult.remaining;

      itemResult = item(remaining);
    }

    if (minCount && result.length < minCount) {
      return new ParserFailure(
        remaining,
        `Expected at least ${minCount} ${description}`,
        result
      );
    }
    if (maxCount && result.length > maxCount) {
      return new ParserFailure(
        remaining,
        `Expected at most ${maxCount} ${description}`,
        result
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
      if (remaining.position.offset === itemResult.remaining.position.offset)
        break;

      result.push(itemResult.value);
      remaining = itemResult.remaining;

      const delimiterResult = delimiter(remaining);
      if (!delimiterResult.success) break;

      itemResult = item(delimiterResult.remaining);
    }

    if (minCount && result.length < minCount) {
      return new ParserFailure(
        remaining,
        `Expected at least ${minCount} ${description}`,
        result
      );
    }
    if (maxCount && result.length > maxCount) {
      return new ParserFailure(
        remaining,
        `Expected at most ${maxCount} ${description}`,
        result
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
  combine: (
    collect: T,
    current: S,
    start: InputPosition,
    end: InputPosition
  ) => T
): Parser<T> {
  return (input: Input) => {
    let result = initial(input);
    if (!result.success) return result;

    let suffixResult = suffix(result.remaining);
    while (suffixResult.success) {
      if (
        suffixResult.remaining.position.offset ===
        result.remaining.position.offset
      )
        break;

      result = new ParserSuccess(
        suffixResult.remaining,
        combine(
          result.value,
          suffixResult.value,
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
  apply: (
    left: T,
    op: OP,
    right: T,
    start: InputPosition,
    end: InputPosition
  ) => T
) {
  const restParser = seq(opParser, operantParser);

  return (input: Input) => {
    const firstResult = operantParser(input);

    if (!firstResult.success) return firstResult;

    let remaining = firstResult.remaining;
    let result = firstResult.value;

    let restResult = restParser(remaining);

    while (restResult.success) {
      if (remaining.position.offset === restResult.remaining.position.offset)
        break;

      var [op, operant] = restResult.value;

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

export function delimitedUntil<T, D, E>(
  itemParser: Parser<T>,
  delimiter: Parser<D>,
  end: Parser<E>,
  description: string,
  recover?: (failure: ParserFailure<T | D>) => ParserSuccess<T>
): Parser<T[]> {
  return (input: Input) => {
    let remaining = input;
    const result: T[] = [];
    let endResult = end(remaining);

    if (endResult.success)
      return new ParserSuccess(endResult.remaining, result);

    while (remaining.available > 0) {
      const itemResult = itemParser(remaining);

      if (!itemResult.success) {
        if (recover !== undefined) {
          const recoverResult = recover(itemResult);
          result.push(recoverResult.value);
          remaining = recoverResult.remaining;
        } else
          return new ParserFailure(
            itemResult.remaining,
            itemResult.expected,
            result
          );
      } else {
        if (remaining.position.offset === itemResult.remaining.position.offset)
          return new ParserFailure(itemResult.remaining, description, result);

        result.push(itemResult.value);
        remaining = itemResult.remaining;
      }

      endResult = end(remaining);
      if (endResult.success)
        return new ParserSuccess(endResult.remaining, result);

      const delimiterResult = delimiter(remaining);
      if (!delimiterResult.success) {
        if (recover !== undefined) {
          const recoverResult = recover(delimiterResult);
          result.push(recoverResult.value);
          remaining = recoverResult.remaining;
        } else
          return new ParserFailure(
            delimiterResult.remaining,
            delimiterResult.expected,
            result
          );
      } else remaining = delimiterResult.remaining;

      endResult = end(remaining);
      if (endResult.success)
        return new ParserSuccess(endResult.remaining, result);
    }

    return new ParserFailure(remaining, endResult.expected, result);
  };
}
