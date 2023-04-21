import { Input, Parser, ParserFailure, ParserSuccess } from ".";

export function then<T, U>(
  first: Parser<T>,
  second: (result: T) => Parser<U>
): Parser<U> {
  return (input: Input) =>
    first(input).select((s) => second(s.value)(s.remaining));
}

export function preceded<T, U>(
  prefix: Parser<T>,
  parser: Parser<U>
): Parser<U> {
  return (input: Input) => prefix(input).select((s) => parser(s.remaining));
}

export function terminated<T, U>(
  parser: Parser<T>,
  suffix: Parser<U>
): Parser<T> {
  return (input: Input) =>
    parser(input).select((s) => suffix(s.remaining).map((_) => s.value));
}

export function between<T, P, S>(
  prefix: Parser<P>,
  parser: Parser<T>,
  suffix: Parser<S>
): Parser<T> {
  return (input: Input) =>
    prefix(input)
      .select((s) => parser(s.remaining))
      .select((s) => suffix(s.remaining).map((_) => s.value));
}

export function seq<P extends any[]>(
  items: [...{ [k in keyof P]: Parser<P[k]> }]
): Parser<P> {
  return (input: Input) => {
    let remaining = input;
    let result: any[] = [];

    for (const item of items) {
      const itemResult = item(remaining);
      if (!itemResult.success)
        return new ParserFailure<P>(remaining, itemResult.expected, undefined);
      result.push(itemResult.value);
      remaining = itemResult.remaining;
    }

    return new ParserSuccess(remaining, result as P);
  };
}
