import { Input, Parser, ParserFailure, ParserSuccess } from ".";

export function preceded<T, U>(
  prefix: Parser<T>,
  parser: Parser<U>
): Parser<U> {
  return (input: Input) => {
    const prefixResult = prefix(input);
    if (!prefixResult.success)
      return new ParserFailure<U>(
        prefixResult.remaining,
        prefixResult.expected,
        undefined
      );
    return parser(prefixResult.remaining);
  };
}

export function terminated<T, U>(
  parser: Parser<T>,
  suffix: Parser<U>
): Parser<T> {
  return (input: Input) => {
    const inputResult = parser(input);
    if (!inputResult.success) return inputResult;
    const suffixResult = suffix(inputResult.remaining);
    if (!suffixResult.success)
      return new ParserFailure(
        suffixResult.remaining,
        suffixResult.expected,
        inputResult.value
      );
    return new ParserSuccess(suffixResult.remaining, inputResult.value);
  };
}

export function between<T, P, S>(
  prefix: Parser<P>,
  parser: Parser<T>,
  suffix: Parser<S>
): Parser<T> {
  return (input: Input) => {
    const prefixResult = prefix(input);
    if (!prefixResult.success)
      return new ParserFailure<T>(
        prefixResult.remaining,
        prefixResult.expected,
        undefined
      );
    const inputResult = parser(prefixResult.remaining);
    if (!inputResult.success) return inputResult;
    const suffixResult = suffix(inputResult.remaining);
    if (!suffixResult.success)
      return new ParserFailure(
        suffixResult.remaining,
        suffixResult.expected,
        inputResult.value
      );
    return new ParserSuccess(suffixResult.remaining, inputResult.value);
  };
}

export function seq<P extends any[]>(
  ...items: { [k in keyof P]: Parser<P[k]> }
): Parser<P> {
  return (input: Input) => {
    let remaining = input;
    let result: any[] = [];

    for (const item of items) {
      const itemResult = item(remaining);
      if (!itemResult.success) {
        if (itemResult.value && result.length == items.length - 1) {
          result.push(itemResult.value);
          return new ParserFailure<P>(
            itemResult.remaining,
            itemResult.expected,
            result as P
          );
        }
        return new ParserFailure<P>(
          itemResult.remaining,
          itemResult.expected,
          undefined
        );
      }
      result.push(itemResult.value);
      remaining = itemResult.remaining;
    }

    return new ParserSuccess(remaining, result as P);
  };
}
